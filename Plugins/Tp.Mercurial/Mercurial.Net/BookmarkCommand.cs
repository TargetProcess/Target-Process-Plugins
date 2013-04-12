using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Mercurial.Attributes;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg bookmark" command (<see href="http://www.selenic.com/mercurial/hg.1.html#bookmark"/>):
    /// track of a line of development with movable markers.
    /// </summary>
    /// <remarks>
    /// Note that in Mercurial, the "bookmark" and "bookmarks" commands are synonyms for each other, which means that
    /// in Mercurial, to get a list of bookmarks, you execute either without arguments, and to create a new bookmark
    /// you execute either with a name argument. In Mercurial.Net I decided to split these two into separate commands,
    /// so that the <see cref="BookmarksCommand"/> lists existing bookmarks, and the <see cref="BookmarkCommand"/>
    /// creates or deletes bookmarks.
    /// </remarks>
    public sealed class BookmarkCommand : MercurialCommandBase<BookmarkCommand>
    {
        /// <summary>
        /// This is the backing field for the <see cref="Name"/> property.
        /// </summary>
        private string _Name = string.Empty;

        /// <summary>
        /// This is the backing field for the <see cref="RenameFrom"/> property.
        /// </summary>
        private string _RenameFrom = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookmarkCommand"/> class.
        /// </summary>
        public BookmarkCommand()
            : base("bookmark")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets the name of the bookmark to create, delete, or rename an existing bookmark to.
        /// Default value is <see cref="string.Empty"/>, but must be set before executing the command.
        /// </summary>
        [DefaultValue("")]
        [NullableArgument]
        public string Name
        {
            get
            {
                return _Name;
            }

            set
            {
                _Name = (value ?? string.Empty).Trim();
            }
        }

        /// <summary>
        /// Sets the value of the <see cref="Name"/> property to the
        /// specified value and returns this <see cref="BookmarkCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The value to set the <see cref="Name"/> property to.
        /// </param>
        /// <returns>
        /// This <see cref="BookmarkCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public BookmarkCommand WithName(string value)
        {
            Name = value;
            return this;
        }

        /// <summary>
        /// Gets or sets the <see cref="Revision"/> to set the bookmark to. If left <c>null</c> will set the
        /// bookmark to the current working folder parent revision.
        /// Default value is <c>null</c>.
        /// </summary>
        [NullableArgument(NonNullOption = "--rev")]
        [DefaultValue(null)]
        public RevSpec Revision
        {
            get;

            set;
        }

        /// <summary>
        /// Sets the value of the <see cref="Revision"/> property to the
        /// specified value and returns this <see cref="BookmarkCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The value to set the <see cref="Revision"/> property to.
        /// </param>
        /// <returns>
        /// This <see cref="BookmarkCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public BookmarkCommand WithRevision(RevSpec value)
        {
            Revision = value;
            return this;
        }

        /// <summary>
        /// Gets or sets the name of the bookmark to rename to <see cref="Name"/>. If left empty, will create a new
        /// bookmark instead of renaming an existing one.
        /// Default value is <see cref="string.Empty"/>.
        /// </summary>
        [DefaultValue("")]
        public string RenameFrom
        {
            get
            {
                return _RenameFrom;
            }

            set
            {
                _RenameFrom = (value ?? string.Empty).Trim();
            }
        }

        /// <summary>
        /// Sets the value of the <see cref="RenameFrom"/> property to the
        /// specified value and returns this <see cref="BookmarkCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The value to set the <see cref="RenameFrom"/> property to.
        /// </param>
        /// <returns>
        /// This <see cref="BookmarkCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public BookmarkCommand WithRenameFrom(string value)
        {
            RenameFrom = value;
            return this;
        }

        /// <summary>
        /// Gets or sets the <see cref="BookmarkAction"/> to take when executing the bookmark command.
        /// Default value is <see cref="BookmarkAction.CreateNew"/>.
        /// </summary>
        [DefaultValue(BookmarkAction.CreateNew)]
        public BookmarkAction Action
        {
            get;
            set;
        }

        /// <summary>
        /// Gets all the arguments to the <see cref="CommandBase{T}.Command"/>, or an
        /// empty array if there are none.
        /// </summary>
        /// <remarks>
        /// Note that as long as you descend from <see cref="MercurialCommandBase{T}"/> you're not required to access
        /// the base property at all, but you are required to return a non-<c>null</c> array reference,
        /// even for an empty array.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// <para><see cref="Action"/> has an invalid value.</para>
        /// </exception>
        public override IEnumerable<string> Arguments
        {
            get
            {
                foreach (string arg in GetBaseArguments())
                    yield return arg;

                switch (Action)
                {
                    case BookmarkAction.CreateNew:
                        break;

                    case BookmarkAction.DeleteExisting:
                        yield return "--delete";
                        break;

                    case BookmarkAction.MoveExisting:
                        yield return "--force";
                        break;

                    case BookmarkAction.RenameExisting:
                        yield return "--rename";
                        yield return RenameFrom;
                        break;

                    default:
                        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unknown BookmarkAction specified for BookmarkCommand: {0}", Action));
                }
            }
        }

        /// <summary>
        /// Gets the base arguments.
        /// </summary>
        /// <returns>
        /// The contents of the base arguments property, to avoide unverifiable code in <see cref="Arguments"/>.
        /// </returns>
        private IEnumerable<string> GetBaseArguments()
        {
            return base.Arguments;
        }

        /// <summary>
        /// Validates the command configuration. This method should throw the necessary
        /// exceptions to signal missing or incorrect configuration (like attempting to
        /// add files to the repository without specifying which files to add.)
        /// </summary>
        /// <remarks>
        /// Note that as long as you descend from <see cref="MercurialCommandBase{T}"/> you're not required to call
        /// the base method at all.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// <para><see cref="Name"/> is empty.</para>
        /// <para>- or -</para>
        /// <para><see cref="Action"/> is <see cref="BookmarkAction.RenameExisting"/> but <see cref="RenameFrom"/> is empty.</para>
        /// </exception>
        public override void Validate()
        {
            base.Validate();

            if (StringEx.IsNullOrWhiteSpace(Name))
                throw new InvalidOperationException("The Name property must be set on the BookmarkCommand before executing it");
            if (Action == BookmarkAction.RenameExisting && StringEx.IsNullOrWhiteSpace(RenameFrom))
                throw new InvalidOperationException("The RenameFrom property must be set on the BookmarkCommand before executing it with an Action of BookmarkAction.RenameExisting");
        }
    }
}