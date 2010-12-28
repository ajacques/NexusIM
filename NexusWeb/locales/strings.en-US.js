Type.registerNamespace("Strings");

Strings.DisableText = "Disable";
Strings.EnableText = "Enable";
Strings.LocationDisabled = "Location sharing is currently disabled.";
Strings.LocationEnabled = "Location sharing is currently enabled.";
Strings.StatusUpdateBoxTip = "How are you today?";
Strings.CommentText = "Reply";
Strings.PluralAge = "{0} years old";
Strings.PluralSecondsAgo = "{0} seconds ago";
Strings.PluralMinutesAgo = "{0} minutes ago";
Strings.PluralHoursAgo = "{0} hours ago";
Strings.PluralDaysAgo = "{0} days ago";
Strings.PluralWeeksAgo = "{0} weeks ago";

String.prototype.format = function()
{
    var formatted = "";
    for(arg in arguments) {
        formatted = this.replace("{" + arg + "}", arguments[arg]);
    }
    return formatted;
};