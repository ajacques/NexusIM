function GetSimpleString(gcity)
{
	var result = "";

	if (gcity.City != "")
		result = SmartAppend(result, gcity.City);
	else if (gcity.AdminLevel2 != null)
		result = SmartAppend(result, gcity.AdminLevel2.Name);

	if (gcity.AdminLevel1 != null)
		result = SmartAppend(result, gcity.AdminLevel1.Name);
	else if (gcity.Country != null)
		result = SmartAppend(result, gcity.Country.ISO3);

	return result;
}

function SmartAppend(base, append)
{
	if (base != "")
		base += ", " + append;
	else
		base += append;

	return base;
}