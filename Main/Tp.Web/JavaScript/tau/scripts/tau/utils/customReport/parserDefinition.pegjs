/* to regenerate use http://pegjs.majda.cz/ */

start
  = expression

expression
=
 name:field ".Count()"i {return "#"+name;}
/ "Count()"i {return "Count";}

/ "sum("i inner:expression ")" {return inner + " Sum"}
/ aggr:aggregation "("i inner:expression ")" {return aggr + " " + inner}


/ group:date "(" field:expression ")" { return {label: field + " by " + group, tickPeriod: group.toLowerCase()}; }
/ complex:complex
/ field:field
/ formula:.+ {return formula.join("")}


field
 = chars:[^- \.()+*/\[\]]+ {return chars.join("")}

complex
 = name:field "." expression:expression { return name + (expression.toLowerCase() == "name" ? "" : (" " + expression)); }

progress
 = chars:[]

date
 = "week"i { return "Week"; }
/ "day"i { return "Day"; }
/ "year"i { return "Year"; }
/ "month"i { return "Month"; }
/ "quarter"i { return "Quarter"; }

aggregation
 = "avg"i { return "Average"; }
/ "max"i { return "Max"; }
/ "min"i { return "Min"; }