#!/usr/bin/perl

use CGI::Carp qw(fatalsToBrowser);
use lib qw(.);
use Bugzilla;
use Bugzilla::Constants;
use Bugzilla::Field;
use Bugzilla::Error;
use Bugzilla::User;
use Bugzilla::Keyword;
use Bugzilla::Bug;
use Bugzilla::BugMail;
use Bugzilla::Search;
use Bugzilla::Status;
use Date::Parse;
use MIME::Base64;
use Data::Dumper;
require Data::Dumper;

Bugzilla->login;
my $user = Bugzilla->login(LOGIN_REQUIRED);

my $cgi = Bugzilla->cgi;
my $bugsListXmlTemplate = 'tp_bugsListXml';
my $scriptVersionXmlTemplate = 'tp_scriptVersionXml';
my $scriptVersion = "2.2.0";
my $supportedBugzillaVersion = '4.0';

# get_bugs
# get_bug_ids

my $cmd = $cgi->param("cmd");

if ($cmd eq 'check')
{
	check();
}
elsif (substr($constants.BUGZILLA_VERSION, 0, 3) eq $supportedBugzillaVersion)
{
	print $cgi->header('text');
	print "BUGZILLA VERSION: " . $constants.BUGZILLA_VERSION . "\n";
	print "SUPPORTED BUGZILLA VERSION: " . $supportedBugzillaVersion . "\n";
	print "SCRIPT VERSION: " . $scriptVersion . "\n";

	if ($cmd eq 'get_bugs')
	{
		get_bugs();
	}
	elsif ($cmd eq 'get_timezone')
	{
		get_timezone();
	}
	elsif ($cmd eq 'get_bug_ids')
	{
		get_bug_ids();
	}
	elsif ($cmd eq 'change_status')
	{
		my $error_mode_cache = Bugzilla->error_mode;
		Bugzilla->error_mode(ERROR_MODE_DIE);
		eval {
			change_status();
		};
		Bugzilla->error_mode($error_mode_cache);
		if ($@) {
			print $@;
			exit;
		}
	}
	elsif($cmd eq 'add_comment')
	{
		add_comment();
	}
	elsif($cmd eq 'assign_user')
	{
		assign_user();
	}
	else
	{
		print $cgi->header('text');
		print "ERROR: Invalid command '".$cmd."'";
	}
}
else
{
	print $cgi->header('text');
	print "ERROR: Bugzilla version '" . $constants.BUGZILLA_VERSION . "' is not supported by 'tp2.cgi'. Please update 'tp2.cgi' and try again.";
}

sub get_timezone
{
	print $cgi->header('text');

	my $dbh = Bugzilla->dbh;
	my $hours = $dbh->selectrow_array('SELECT TIMEDIFF(CURTIME(),UTC_TIME())', undef);

	print $hours;
}

sub add_comment
{
	print $cgi->header('text');

	my $bugId = $cgi->param("bugid");

	my $bug = new Bugzilla::Bug($bugId);
	if (!defined($bug) || $bug->bug_id ne $bugId)
	{
		print "Bug ID=".$bugId." not found";
		exit;
	}

	my $comment_text = decode_base64($cgi->param("comment_text"));

	my $owner = $cgi->param('owner');
	my $date = $cgi->param('date') =~ m/^([Z0-9\- :]+)$/ ? $1 : die "Invalid date: ".$cgi->param('date');

	my $ownerid = 0;
	my $dbh = Bugzilla->dbh;

	if ($owner eq '')
	{
		$ownerid = $user->id;
		$comment_text = $comment_text."\n\rThis comment was added from TargetProcess";
	}
	else
	{
		$owner = $owner =~ m/^([a-zA-Z0-9\._@]+)$/ ? $1 : die "Invalid owner: ".$cgi->param('owner');

		my $userids = $dbh->selectcol_arrayref("SELECT userid FROM profiles WHERE login_name = ?", undef, $owner);

		if(@$userids < 1)
		{
			$ownerid = $user->id;
			$comment_text = $comment_text."\n\rThis comment was added from TargetProcess by ".$owner;
		}
		else
		{
			$ownerid = @$userids[0];
		}
	}

	$dbh->do("INSERT INTO longdescs (bug_id, who, thetext, bug_when)
                       VALUES (?,?,?,?)", undef,
                 $bug->id, $ownerid, $comment_text, $date);

	$bug->{added_comments} = [];
	$bug->update();

	print 'OK';
}

sub assign_user
{
	print $cgi->header('text');

	my $bugId = $cgi->param("bugid");
	my $localUser = $cgi->param("user");

	my $bug = new Bugzilla::Bug($bugId);
	if (!defined($bug) || $bug->bug_id ne $bugId)
	{
		print "Bug ID=".$bugId." not found";
		exit;
	}

	if ($localUser eq '')
	{
		$localUser = $user;
	}

	$bug->set_assigned_to($localUser);
	$bug->update();

	print 'OK';
}

sub change_status
{
	print $cgi->header('text');

	my $id = $cgi->param('id');

	#ValidateBugID($id);

	my $bug = new Bugzilla::Bug($id);
	if (!defined($bug) || $bug->bug_id ne $id)
	{
		print "Bug ID=".$id." not found";
		exit;
	}

	$cgi->param('resolution') || $cgi->param('resolution', 'FIXED');
	#$cgi->param('resolution', uc($cgi->param('resolution')));

	check_resolution();

	my %set_all_fields = ();
	$set_all_fields{'bug_status'} = $cgi->param('status');
	$set_all_fields{'resolution'} = $cgi->param('resolution');
	$set_all_fields{'dup_id'} = $cgi->param('dup_id');
	my $new_status = Bugzilla::Status->check($cgi->param('status'));
	if($new_status->comment_required_on_change_from($bug->status)) {
		$set_all_fields{'comment'} = {body => "Status changed from ".$bug->status->name." to ".$new_status->name." by Targetprocess Bugzilla Plugin", is_private => 0};
	}

	$bug->set_all(\%set_all_fields);
#	$bug->set_status(scalar $cgi->param('status'),{
#				resolution => scalar $cgi->param('resolution'),
#				dupe_of => scalar $cgi->param('dup_id')
#				}
#			);
	my $changes = $bug->update();
	if ($changes->{'bug_status'}) {
		send_changes($bug, $changes, $vars);
	}

	print 'OK';
}

sub send_changes
{
	my ($self, $changes, $vars) = @_;
	my $user = Bugzilla->user;
	my $old_qa  = $changes->{'qa_contact'} ? $changes->{'qa_contact'}->[0] : '';
	my $old_own = $changes->{'assigned_to'} ? $changes->{'assigned_to'}->[0] : '';
	my $old_cc  = $changes->{cc} ? $changes->{cc}->[0] : '';
	my %forced = (
		cc        => [split(/[,;]+/, $old_cc)],
		owner     => $old_own,
		qacontact => $old_qa,
		changer   => $user,
	);
	_send_bugmail({ id => $self->id, type => 'bug', forced => \%forced }, $vars);

	# If there were changes in dependencies, we need to notify those
	# dependencies.
	my %notify_deps;

	if ($changes->{'bug_status'}) {
		my ($old_status, $new_status) = @{ $changes->{'bug_status'} };
		# If this bug has changed from opened to closed or vice-versa,
		# then all of the bugs we block need to be notified.
		if (is_open_state($old_status) ne is_open_state($new_status)) {
			$notify_deps{$_} = 1 foreach (@{ $self->blocked });
		}
	}
	# To get a list of all changed dependencies, convert the "changes" arrays
	# into a long string, then collapse that string into unique numbers in
	# a hash.
	my $all_changed_deps = join(', ', @{ $changes->{'dependson'} || [] });
	$all_changed_deps = join(', ', @{ $changes->{'blocked'} || [] }, $all_changed_deps);
	my %changed_deps = map { $_ => 1 } split(', ', $all_changed_deps);
	# When clearning one field (say, blocks) and filling in the other
	# (say, dependson), an empty string can get into the hash and cause
	# an error later.
	delete $changed_deps{''};

	my %all_dep_changes = (%notify_deps, %changed_deps);
	foreach my $id (sort { $a <=> $b } (keys %all_dep_changes)) {
		_send_bugmail({ forced => { changer => $user }, type => "dep", id => $id }, $vars);
	}
}

sub _send_bugmail
{
	my ($params, $vars) = @_;
	my $results = Bugzilla::BugMail::Send($params->{'id'}, $params->{'forced'});
}

sub check_resolution
{
	# Check here, because it's the only place we require the resolution
	my $resolution = $cgi->param('resolution');
	my $found = 0;
	my $bugId = $cgi->param('id');
	my $bug = new Bugzilla::Bug($bugId);

	my $settable_resolutions = $bug->choices->{resolution};

	foreach my $i (@$settable_resolutions) {
		if ($i->{value} eq $resolution) {
			$found++;
		}
	}
	if (!$found) {
		print "Resolution '".$resolution."' not found";
		exit;
	}
	#check_field('resolution', scalar $cgi->param('resolution'),
	#            Bugzilla::Bug->settable_resolutions);

	# don't resolve as fixed while still unresolved blocking bugs
	if (Bugzilla->params->{"noresolveonopenblockers"} && $resolution eq 'FIXED'
		&& (!$bug->resolution || $resolution ne $bug->resolution)
		&& scalar @{$bug->dependson})
	{
		my @dependencies = Bugzilla::Bug->new_from_list($bug->dependson);
		my $count_open = grep { $_->isopened } @dependencies;
		if ($count_open) {
			print "Can't set resolution to '$resolution'. Bugzilla Bug#$bugId still has $count_open ";
			if ($count_open == 1) {
				print "dependency.";
			}
			else {
				print "dependencies.";
			}
			exit;
		}
	}
}

sub check
{
	if ($user && $user->login())
	{
		print $cgi->header('xml');
		my $template = Bugzilla->template;
		addScriptVersionTemplate($template);

		my $vars = {};
		foreach my $id ($cgi->param('id')) {
			my @ids = split(/,/, $id);
			foreach (@ids) {
				my $bug = new Bugzilla::Bug($_);
				if (!$bug->{error} && !$user->can_see_bug($bug->bug_id)) {
					$bug->{error} = 'NotPermitted';
				}
				push(@bugs, $bug);
			}
		}

		eval {
			my @customFields = get_custom_field_names();
			$vars->{'custom_field_names'} = \@customFields;
		};

		eval {
			$vars->{'scriptVersion'}  = $scriptVersion;
		};

		eval {
			$vars->{'supportedBugzillaVersion'} = $supportedBugzillaVersion;
		};

		eval {
			$vars->{'severity_names'} =   get_legal_field_values('bug_severity');
		};

		eval {
			$vars->{'priority_names'} =   get_legal_field_values('priority');
		};

		eval {
			$vars->{'status_names'} = [Bugzilla::Status->get_all];
		};

		eval {
			my $fieldvalues = Bugzilla->dbh->selectall_arrayref("SELECT value AS name"
				. "  FROM resolution ORDER BY sortkey",
				{Slice =>{}});
			$vars->{'resolutions'} = $fieldvalues;
		};

		$vars->{'timezone'} = $user->timezone->name;

		$template->process($scriptVersionXmlTemplate , $vars) || die $template->error();
	}
}

sub get_custom_field_names {
	# Get a list of custom fields and convert it into a list of their names.
	my @customFields =  map($_, @{Bugzilla::Field->match({ custom=>1, obsolete=>0 })});

	foreach $customField (@customFields)
	{
		my $field = $customField->{'name'};
		if ($customField->{'type'} == FIELD_TYPE_SINGLE_SELECT ||
		$customField->{'type'} == FIELD_TYPE_MULTI_SELECT) {
			my $fieldvalues = Bugzilla->dbh->selectall_arrayref("SELECT value AS name, sortkey"
				. "  FROM $field ORDER BY sortkey, value",
				{Slice =>{}});
			$customField->{'legals'} = $fieldvalues;
		}
	}
	return @customFields;
}

sub get_bugs {
	my $template = Bugzilla->template;
	addXmlTemplate($template);

	print $cgi->header('xml');

	my $vars = {};
	foreach my $id ($cgi->param('id')) {
		my @ids = split(/,/, $id);
		foreach (@ids) {
			my $bug = Bugzilla::Bug->check($_);
			if (!$bug->{error} && !$user->can_see_bug($bug->bug_id)) {
				$bug->{error} = 'NotPermitted';
			}
			push(@bugs, $bug);
		}
	}
	$vars->{'bugs'} = \@bugs;
	my @bugids = map {$_->bug_id} @bugs;
	$vars->{'bugids'} = join(", ", @bugids);

	my @fieldlist = (Bugzilla::Bug->fields, 'group', 'long_desc', 'attachment', 'attachmentdata');
	foreach (@fieldlist) {
		$displayfields{$_} = 1;
	}

	$vars->{'displayfields'} = \%displayfields;
	my @customFields = get_custom_field_names();

	#my $field = $vars->{'field'}->name;

	$vars->{'custom_field_names'} = \@customFields;
	$template->process($bugsListXmlTemplate, $vars) || die $template->error();
}

sub get_bug_ids
{
	print $cgi->header('text');

	my $name = $cgi->param("name");
	my $date = $cgi->param("date");

	my @missingQueries = ();
	my @bugIds = ();
	my @subqueries = split(/,/, $name);
	my @args = ($cgi->param("name"));
	my $dbh = Bugzilla->dbh;
	foreach my $queryName (@subqueries) {
		my $query = findQuery($queryName);

		if ($query) {
			my $params =  new Bugzilla::CGI($query->url);
			if ($date) {
				my $fromdate = $params->param("chfieldfrom");
				if($fromdate){
					my $t1 = str2time($fromdate);
					my $t2 = str2time($date);
					if($t1 > $t2) {
						$date = $fromdate;
					}
				}
				$params->param("chfieldfrom", $date);
				$params->param("chfieldto", "Now");
			}

			my $search = new Bugzilla::Search('fields' => ['bug_id'], 'params' => $params);
			my $sql = $search->getSQL();
			my $dbh = Bugzilla->dbh;
			my $sth = $dbh->prepare($sql);

			$sth->execute();

			while (my @row = $sth->fetchrow_array()) {
				unshift @bugIds, $row[0];
			}
		}
		else
		{
			push(@missingQueries, $queryName);
		}
	}
	if (@missingQueries > 0)
	{
		print "ERROR: query not found: " . join(',', @missingQueries);
	}
	else
	{
		print join(',', @bugIds);
	}
}

sub findQuery {
	my $queryName = shift;
	$queryName =~ s/^\s+//;
	$queryName =~ s/\s+$//;

	my $list = Bugzilla->user->queries();

	foreach my $query (@$list) {
		return $query if ($query->name eq $queryName);
	}
}

sub addScriptVersionTemplate{
   my $template = shift;
   my $scriptVersionXmlTemplate_text = <<'SCRIPTVERSION';
   <?xml version="1.0" [% IF Param('utf8') %]encoding="UTF-8" [% END %]standalone="yes" ?>
<!DOCTYPE bugzilla_properties SYSTEM "[% Param('urlbase') %]bugzilla.dtd">

<bugzilla_properties version="[% constants.BUGZILLA_VERSION %]"
          urlbase="[% Param('urlbase') %]"
          maintainer="[% Param('maintainer') FILTER xml %]"
[% IF user.id %]
          exporter="[% user.email FILTER xml %]"
[% END %]
>
  <script_version>[% scriptVersion %]</script_version>
  <supported_bugzilla_version>[% supportedBugzillaVersion %]</supported_bugzilla_version>
  <timezone>valid</timezone>
  <custom_fields>
  [% FOREACH cf = custom_field_names %]
     <name>[% cf.name %]</name>
  [% END %]
  </custom_fields>
  <severities>
  [% FOREACH name = severity_names  %]
     <name>[% name %]</name>
  [% END %]
  </severities>
  <priorities>
  [% FOREACH name = priority_names  %]
     <name>[% name %]</name>
  [% END %]
  </priorities>
  <statuses>
  [% FOREACH status = status_names  %]
     <name>[% status.${'value'}  FILTER xml %]</name>
  [% END %]
  </statuses>
  <resolutions>
  [% FOREACH resolution = resolutions  %]
     <name>[% resolution.${'name'}  FILTER xml %]</name>
  [% END %]
  </resolutions>
  </bugzilla_properties>
SCRIPTVERSION
     $template->context->define_block($scriptVersionXmlTemplate, $scriptVersionXmlTemplate_text) || die "Can't add template block";
}

sub addXmlTemplate {
    my $template = shift;
my $bugsListXmlTemplate_text = <<'BUGSLIST';
[% PROCESS bug/time.html.tmpl %]
[% PROCESS "global/field-descs.none.tmpl" %]
<?xml version="1.0" [% IF Param('utf8') %]encoding="UTF-8" [% END %]standalone="yes" ?>
<!DOCTYPE bugzilla SYSTEM "[% Param('urlbase') %]bugzilla.dtd">

<bugzilla version="[% constants.BUGZILLA_VERSION %]"
          urlbase="[% Param('urlbase') %]"
          maintainer="[% Param('maintainer') FILTER xml %]"
[% IF user.id %]
          exporter="[% user.email FILTER xml %]"
[% END %]
>
[% FOREACH bug = bugs %]
  [% IF bug.error %]
    <bug error="[% bug.error FILTER xml %]">
      <bug_id>[% bug.bug_id FILTER xml %]</bug_id>
    </bug>
  [% ELSE %]
    <bug>
      [% FOREACH field = bug.fields %]
        [% IF displayfields.$field %]
          [%+ PROCESS bug_field %]
        [% END %]
      [% END %]

      [%# Now handle 'special' fields #%]
      [% IF displayfields.group %]
        [% FOREACH g = bug.groups %]
          [% NEXT UNLESS g.ison %]
          <group>[% g.name FILTER xml %]</group>
        [% END %]
      [% END %]

      [%# Bug Flags %]
      [% FOREACH type = bug.flag_types %]
        [% FOREACH flag = type.flags %]
          <flag name="[% type.name FILTER xml %]"
                status="[% flag.status FILTER xml %]"
                setter="[% flag.setter.login FILTER xml %]"
          [% IF flag.requestee %]
              requestee="[% flag.requestee.login FILTER xml %]"
          [% END %]
          />
        [% END %]
      [% END %]
      [% IF displayfields.long_desc %]
        [% FOREACH c = bug.comments %]
          [% NEXT IF c.isprivate && !user.in_group(Param("insidergroup")) %]
          <long_desc isprivate="[% c.isprivate FILTER xml %]">
            [% IF c.author %]
               <who name="[% c.author.name FILTER xml %]">[% c.author.email FILTER xml %]</who>
            [% ELSE %]
               <who name="[% c.name FILTER xml %]">[% c.email FILTER xml %]</who>
            [% END %]
            <bug_when>[% c.creation_ts FILTER time FILTER xml %]</bug_when>
            [% IF user.in_group(Param('timetrackinggroup')) && (c.work_time - 0 != 0) %]
              <work_time>[% PROCESS formattimeunit time_unit = c.work_time FILTER xml %]</work_time>
            [% END %]
            <thetext>[% c.body FILTER xml %]</thetext>
          </long_desc>
        [% END %]
      [% END %]

      [% IF displayfields.attachment %]
        [% FOREACH a = bug.attachments %]
          [% NEXT IF a.isprivate && !user.in_group(Param("insidergroup")) %]
          <attachment
              isobsolete="[% a.isobsolete FILTER xml %]"
              ispatch="[% a.ispatch FILTER xml %]"
              isprivate="[% a.isprivate FILTER xml %]"
          >
            <attachid>[% a.id %]</attachid>
            <date>[% a.attached FILTER time FILTER xml %]</date>
            <desc>[% a.description FILTER xml %]</desc>
            <filename>[% a.filename FILTER xml %]</filename>
            <type>[% a.contenttype FILTER xml %]</type>
            <size>[% a.datasize FILTER xml %]</size>
        [% IF a.attacher %]
            <attacher>[% a.attacher.login_name%]</attacher>
        [% END %]
        [% IF displayfields.attachmentdata %]
            <data encoding="base64">[% a.data FILTER base64 %]</data>
        [% END %]

            [% FOREACH flag = a.flags %]
              <flag name="[% flag.type.name FILTER xml %]"
                    status="[% flag.status FILTER xml %]"
                    setter="[% flag.setter.email FILTER xml %]"
                    [% IF flag.status == "?" && flag.requestee %]
                      requestee="[% flag.requestee.email FILTER xml %]"
                    [% END %]
               />
            [% END %]
          </attachment>
        [% END %]
      [% END %]
      [%+ PROCESS bug_custom_field %]

    </bug>
  [% END %]
[% END %]

</bugzilla>

[% BLOCK bug_field %]
  [% FOREACH val = bug.$field %]
    [%# We need to handle some fields differently. This should become
      # nicer once we have custfields, and a type attribute for the fields
      #%]
    [% name = '' %]
    [% IF field == 'reporter' OR field == 'assigned_to' OR
          field == 'qa_contact' %]
      [% name = val.name %]
      [% val = val.email %]
    [% ELSIF field == 'creation_ts' OR field == 'delta_ts' %]
      [% val = val  FILTER time %]
    [% END %]
    <[% field %][% IF name != '' %] name="[% name FILTER xml %]"[% END -%]>[% val FILTER xml %]</[% field %]>
  [% END %]
[% END %]

[% BLOCK bug_custom_field %]
  [% FOREACH cf = custom_field_names %]
        <custom_field>
            <cf_name>[% cf.name %]</cf_name>
                        <cf_value>[% bug.${cf.name} FILTER xml %]</cf_value>
                        <cf_type>[% field_types.${cf.type} %]</cf_type>
                        <cf_description>[% cf.description %]</cf_description>
                    <cf_values>
                      [% FOREACH value = bug.${cf.name}  %]
                        <cf_value>[% value FILTER xml%]</cf_value>
                      [% END %]
                        </cf_values>
                    <cf_legal_values>
                          [% FOREACH  legal_value = cf.${'legals'}  %]
                            <cf_value>[% legal_value.${'name'} FILTER xml %]</cf_value>
                          [% END %]
                        </cf_legal_values>
        </custom_field>
  [% END %]
[% END %]
BUGSLIST

	$template->context->define_block($bugsListXmlTemplate, $bugsListXmlTemplate_text) || die "Can't add template block";
}