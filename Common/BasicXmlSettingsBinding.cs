using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Security.Cryptography;

namespace InstantMessage
{
	internal class SettingDataStruct
	{
		public SettingDataStruct() {}
		public SettingDataStruct(SettingAttributes attribs, bool mIsBad)
		{
			attributes = attribs;
			isBad = mIsBad;
		}
		public SettingDataStruct(SettingAttributes attribs, string mValue)
		{
			attributes = attribs;
			value = mValue;
		}
		public SettingAttributes attributes = SettingAttributes.None;
		public string value = "";
		public bool isBad = false; // Is this bad data?
	}
	// This is the new style for settings.. provides abstraction
	public class BasicXmlSettingsBinding : ISettings
	{
		public BasicXmlSettingsBinding(string mXmlSavePath)
		{
			xmlpath = mXmlSavePath;
			mProtocols = new List<IMProtocol>();
		}
		/// <summary>
		/// Loads all configuration settings and accounts found in a specified Xml file
		/// </summary>
		/// <exception cref="System.Xml.XmlException">Thrown if there is a parse error with the file.</exception>
		public void Load()
		{
			if (File.Exists(xmlpath)) // Yeah this right here
			{
				XmlDocument xml = new XmlDocument();

				// All this stuff is just trying to get the file handle when handling exceptions
				FileStream stream = null;
				try	{
					stream = new FileStream(xmlpath, FileMode.Open);
					xml.Load(stream);
				} catch (IOException e) {
					e = e;
				} catch (XmlException e) {
					stream.Close();
					throw e;
				} finally {
					stream.Close();
				}

				if (xml.DocumentElement == null)
					return;

				LoadAccounts(xml);
				LoadGeneralSettings(xml);
				LoadSettingLists(xml);
				LoadContactSettings(xml);

				mLoaded = true;
				if (onFileLoad != null)
					onFileLoad(this, null);
			}
		}
		
		public void Save()
		{
			XmlDocument xml = new XmlDocument();

			xml.AppendChild(xml.CreateXmlDeclaration("1.0", "utf-8", String.Empty));

			XmlElement root = xml.CreateElement("root");

			// Save all protocols
			XmlElement accounts = xml.CreateElement("accounts");
			SaveAccounts(xml, accounts);
			root.AppendChild(accounts);

			XmlElement contacts = xml.CreateElement("contacts");

			SaveContactSettings(xml, contacts);

			root.AppendChild(contacts);

			XmlElement settings = xml.CreateElement("settings");

			SaveGeneralSettings(xml, settings);
			SaveSettingLists(xml, settings);

			root.AppendChild(settings);
			xml.AppendChild(root);

			xml.Save(xmlpath);
		}

		private void SaveSettingLists(XmlDocument xml, XmlElement settings)
		{
			foreach (KeyValuePair<string, List<string>> lpair in customLists)
			{
				XmlElement setting = xml.CreateElement("settinglist");

				setting.SetAttribute("name", lpair.Key);

				foreach (string subitem in lpair.Value)
				{
					XmlElement xsetting = xml.CreateElement("subitem");

					xsetting.InnerText = subitem;

					setting.AppendChild(xsetting);
				}

				settings.AppendChild(setting);
			}
		}
		private void SaveGeneralSettings(XmlDocument xml, XmlElement settings)
		{
			foreach (KeyValuePair<string, string> gpair in generalSettings)
			{
				XmlElement setting = xml.CreateElement("setting");

				setting.SetAttribute("name", gpair.Key);
				setting.InnerText = gpair.Value;

				settings.AppendChild(setting);
			}
		}
		private void SaveContactSettings(XmlDocument xml, XmlElement contacts)
		{
			foreach (KeyValuePair<IMBuddy, Dictionary<string, string>> cpair in contactSettings)
			{
				XmlElement contact = xml.CreateElement("contact");

				contact.SetAttribute("username", cpair.Key.Username);
				contact.SetAttribute("protocol", cpair.Key.Protocol.ShortProtocol);
				contact.SetAttribute("account", cpair.Key.Protocol.Username);

				foreach (KeyValuePair<string, string> spair in cpair.Value)
				{
					XmlElement setting = xml.CreateElement("setting");

					setting.SetAttribute("name", spair.Key);

					setting.InnerText = spair.Value;

					contact.AppendChild(setting);
				}
				contacts.AppendChild(contact);
			}

			foreach (KeyValuePair<string, Dictionary<string, string>> cpair in contactBackupSettings)
			{
				XmlElement contact = xml.CreateElement("contact");

				string[] keys = cpair.Key.Split('-');

				contact.SetAttribute("username", keys[0]);
				contact.SetAttribute("account", keys[1]);
				contact.SetAttribute("protocol", keys[2]);

				foreach (KeyValuePair<string, string> spair in cpair.Value)
				{
					XmlElement setting = xml.CreateElement("setting");

					setting.SetAttribute("name", spair.Key);

					setting.InnerText = spair.Value;

					contact.AppendChild(setting);
				}
				contacts.AppendChild(contact);
			}
		}
		private void SaveAccounts(XmlDocument xml, XmlElement accounts)
		{
			foreach (IMProtocol protocol in mProtocols)
			{
				if (!protocol.EnableSaving)
					continue;

				XmlElement elem = xml.CreateElement("account");
				XmlElement protE = xml.CreateElement("protocol");
				XmlElement usr = xml.CreateElement("username");
				XmlElement pass = xml.CreateElement("password");
				XmlElement enable = xml.CreateElement("enabled");
				XmlElement guidElem = xml.CreateElement("guid");

				XmlText prot2 = xml.CreateTextNode(protocol.ShortProtocol.ToLower());
				XmlText usr2 = xml.CreateTextNode(protocol.Username);
				XmlText pass2 = null;
				XmlText enable2 = xml.CreateTextNode(System.Convert.ToString(protocol.Enabled));
				XmlText guidText = xml.CreateTextNode(protocol.Guid.ToString());

				if (protocol.SavePassword)
				{
					pass2 = xml.CreateTextNode(protocol.Password);
					pass.AppendChild(pass2);

					pass.SetAttribute("crc", computeCrc32Hash(protocol.Password));
				}

				protE.AppendChild(prot2);
				usr.AppendChild(usr2);
				enable.AppendChild(enable2);
				guidElem.AppendChild(guidText);
				elem.AppendChild(protE);
				elem.AppendChild(usr);
				elem.AppendChild(pass);
				elem.AppendChild(guidElem);
				elem.AppendChild(enable);

				if (protocolSettings.ContainsKey(protocol))
				{
					foreach (KeyValuePair<string, SettingDataStruct> psetting in protocolSettings[protocol])
					{
						XmlElement setting = xml.CreateElement("setting");

						setting.SetAttribute("name", psetting.Key);

						if ((psetting.Value.attributes & SettingAttributes.Hashed) != 0)
							setting.SetAttribute("crc", computeCrc32Hash(psetting.Value.value));

						setting.InnerText = psetting.Value.value;

						elem.AppendChild(setting);
					}
				}

				accounts.AppendChild(elem);
			}
		}
		
		private void LoadContactSettings(XmlDocument xml)
		{
			if (xml.DocumentElement["contacts"] == null)
				return;

			XmlNodeList conSettings = xml.DocumentElement["contacts"].GetElementsByTagName("contact", "");

			foreach (XmlNode setting in conSettings)
			{
				if (setting is XmlElement)
				{
					var s = setting as XmlElement;

					var protocol = from IMProtocol p in mProtocols where p.Username == s.GetAttribute("account") && p.ShortProtocol == s.GetAttribute("protocol") select new { p };

					if (protocol.Count() == 0)
						continue;

					var buddy = from IMBuddy b in protocol.First().p where b.Username == s.GetAttribute("username") select new { b };

					Dictionary<string, string> cdict = new Dictionary<string, string>();
					foreach (XmlNode csetting in setting)
					{
						if (csetting is XmlElement)
						{
							var c = csetting as XmlElement;
							cdict.Add(c.GetAttribute("name"), c.InnerText);
						}
					}

					if (buddy.Count() >= 1) // If false.. timing issues! TODO: FIX ME
						contactSettings.Add(buddy.First().b, cdict);
					else {
						string buddyname = s.GetAttribute("username") + "-" + s.GetAttribute("account") + "-" + s.GetAttribute("protocol");
						contactBackupSettings.Add(buddyname, cdict);
					}
				}
			}
		}
		private void LoadSettingLists(XmlDocument xml)
		{
			if (xml.DocumentElement["setting"] == null)
				return;

			XmlNodeList genLists = xml.DocumentElement["settings"].GetElementsByTagName("settinglist", "");

			foreach (XmlNode setting in genLists)
			{
				if (setting is XmlElement)
				{
					var s = setting as XmlElement;
					List<string> templist = new List<string>();
					foreach (XmlNode subitem in s)
					{
						if (subitem is XmlElement)
						{
							var si = subitem as XmlElement;
							templist.Add(si.InnerText);
						}
					}
					customLists.Add(s.GetAttribute("name"), templist);
				}
			}
		}
		private void LoadGeneralSettings(XmlDocument xml)
		{
			if (xml.DocumentElement["settings"] == null)
				return;

			XmlNodeList genSettings = xml.DocumentElement["settings"].GetElementsByTagName("setting", "");

			foreach (XmlNode setting in genSettings)
			{
				if (setting is XmlElement)
				{
					var s = setting as XmlElement;
					generalSettings.Add(s.GetAttribute("name"), s.InnerText);
				}
			}
		}
		private void LoadAccounts(XmlDocument xml)
		{
			if (xml.DocumentElement["accounts"] == null)
				return;

			XmlNodeList accounts = xml.DocumentElement["accounts"].GetElementsByTagName("account", "");
			foreach (XmlNode account in accounts)
			{
				var acc = account as XmlElement;
				IMProtocol protocol = IMProtocol.FromString(account["protocol"].InnerText);
				if (protocol != null)
				{
					if (account["username"] != null)
						protocol.Username = account["username"].InnerText;

					if (account["password"] != null)
						protocol.Password = account["password"].InnerText;

					try	{
						protocol.Guid = new Guid(account["guid"].InnerText);
					} catch (NullReferenceException) {
						protocol.Guid = Guid.NewGuid();
						XmlElement elem = xml.CreateElement("guid");
					}

					protocol.Status = IMStatus.OFFLINE;

					if (account["enabled"] != null)
						protocol.Enabled = Convert.ToBoolean(account["enabled"].InnerText);

					XmlNodeList pSettings = acc.GetElementsByTagName("setting", "");
					Dictionary<string, SettingDataStruct> pSettingDb = new Dictionary<string, SettingDataStruct>();

					foreach (XmlNode setting in pSettings)
					{
						if (setting is XmlElement)
						{
							var s = setting as XmlElement;

							SettingDataStruct dstruct = new SettingDataStruct();
							dstruct.value = s.InnerText;

							if (s.HasAttribute("crc"))
							{
								dstruct.attributes = dstruct.attributes | SettingAttributes.Hashed;
								if (s.GetAttribute("crc") != computeCrc32Hash(dstruct.value))
									dstruct.isBad = true;
							}

							if (s.HasAttribute("encrypted"))
							{
								dstruct.attributes = dstruct.attributes | SettingAttributes.Encrypted;
								// Decrypt it?
							}

							pSettingDb.Add(s.GetAttribute("name"), dstruct);
						}
					}

					protocolSettings.Add(protocol, pSettingDb);

					if (!mProtocols.Contains(protocol))
						mProtocols.Add(protocol);
				}
			}
		}

		public bool AutoSave
		{
			get {
				return mAutoSave;
			}
			set {
				mAutoSave = value;
			}
		}
		public bool IsLoaded
		{
			get {
				return mLoaded;
			}
		}
		public List<IMProtocol> Accounts
		{
			get {
				return mProtocols;
			}
			set {
				mProtocols = value;
			}
		}
		public Dictionary<IMBuddy, Dictionary<string, string>> ContactSettings
		{
			get {
				return contactSettings;
			}
		}
		public Dictionary<string, Dictionary<string, string>> ContactBackupSettings
		{
			get {
				return contactBackupSettings;
			}
		}

		public event EventHandler onFileLoad;

		public void SetAccountSetting(IMProtocol account, string setting, string value)
		{
			SetAccountSetting(account, setting, value, SettingAttributes.None);
		}
		public void SetAccountSetting(IMProtocol account, string setting, string value, SettingAttributes attrib)
		{
			if (!protocolSettings.ContainsKey(account))
				protocolSettings.Add(account, new Dictionary<string, SettingDataStruct>());

			SettingDataStruct dstruct = new SettingDataStruct(attrib, value);

			if (protocolSettings[account].ContainsKey(setting))
				protocolSettings[account][setting] = dstruct;
			else
				protocolSettings[account].Add(setting, dstruct);

			if (mAutoSave)
				Save();
		}
		public void SetContactSetting(IMBuddy buddy, string setting, string value)
		{
			if (!contactSettings.ContainsKey(buddy))
				contactSettings.Add(buddy, new Dictionary<string, string>());

			if (contactSettings[buddy].ContainsKey(setting))
				contactSettings[buddy][setting] = value;
			else
				contactSettings[buddy].Add(setting, value);

			if (mAutoSave)
				Save();
		}
		public void SetContactSetting(string username, IMProtocol protocol, string setting, string value)
		{
			var buddy = from IMBuddy b in protocol where b.Username == username select new { b };
			if (buddy.Count() >= 1)
			{
				var b = buddy.First().b;
				if (contactSettings[b].ContainsKey(setting))
					contactSettings[b][setting] = value;
				else
					contactSettings[b].Add(setting, value);

				if (mAutoSave)
					Save();
			}
		}
		public void SetCustomSetting(string setting, string value)
		{
			if (generalSettings.ContainsKey(setting))
				generalSettings[setting] = value;
			else
				generalSettings.Add(setting, value);

			if (mAutoSave)
				Save();
		}
		public void SetSettingList(string name, List<string> list)
		{
			if (customLists.ContainsKey(name))
				customLists[name] = list;
			else
				customLists.Add(name, list);

			if (mAutoSave)
				Save();
		}

		public void DeleteAccountSetting(IMProtocol account, string setting)
		{
			if (protocolSettings.ContainsKey(account) && protocolSettings[account].ContainsKey(setting))
			{
				protocolSettings[account].Remove(setting);

				if (mAutoSave)
					Save();
			}
		}
		public void DeleteContactSetting(IMBuddy buddy, string setting)
		{
			if (contactSettings.ContainsKey(buddy) && contactSettings[buddy].ContainsKey(setting))
			{
				contactSettings[buddy].Remove(setting);

				if (mAutoSave)
					Save();
			}
		}
		public void DeleteContactSetting(string username, IMProtocol protocol, string setting)
		{
			var buddy = from IMBuddy b in protocol where b.Username == username select new { b };

			if (buddy.Count() >= 1)
			{
				contactSettings[buddy.First().b].Remove(setting);
			}
		}
		public void DeleteCustomSetting(string setting)
		{
			if (generalSettings.ContainsKey(setting))
			{
				generalSettings.Remove(setting);

				if (mAutoSave)
					Save();
			}
		}
		public string GetAccountSetting(IMProtocol account, string setting, string defaultValue)
		{
			if (protocolSettings.ContainsKey(account) && protocolSettings[account].ContainsKey(setting))
			{
				SettingDataStruct dstruct = protocolSettings[account][setting];

				if (dstruct.isBad)
					throw new HashMismatchException("Setting failed hash test. Tamper detection.");

				return dstruct.value;
			} else {
				return defaultValue;
			}
		}
		public void DeleteSettingList(string list)
		{
			if (customLists.ContainsKey(list))
				customLists.Remove(list);

			if (mAutoSave)
				Save();
		}
		/// <remarks>
		/// This has the possibility to have unexpected return values if there are multiple contacts with the same username on the same account.
		/// </remarks>
		public string GetContactSetting(string userName, IMProtocol protocol, string setting, string defaultValue)
		{
			// This stuff checks to see if this user was non-existent at the time of loading this configuration file
			string buddyname = userName + "-" + protocol.Username + "-" + protocol.ShortProtocol;
			if (contactBackupSettings.ContainsKey(buddyname))
			{
				Dictionary<string, string> backup = contactBackupSettings[buddyname];
				var buddies = from IMBuddy b in protocol where b.Username == userName select new { b };
				if (buddies.Count() >= 1)
				{
					contactSettings.Add(buddies.First().b, backup);
					contactBackupSettings.Remove(buddyname);
					if (backup.ContainsKey(setting))
						return backup[setting];
				}
			}

			var buddy = from IMBuddy b in protocol where b.Username == userName select new { b };
			if (buddy.Count() >= 1 && contactSettings[buddy.First().b].ContainsKey(setting))
				return contactSettings[buddy.First().b][setting];
			else
				return defaultValue;
		}
		public string GetContactSetting(IMBuddy buddy, string setting, string defaultValue)
		{
			string buddyname = buddy.Username + "-" + buddy.Protocol.Username + "-" + buddy.Protocol.ShortProtocol;
			if (contactBackupSettings.ContainsKey(buddyname))
			{
				Dictionary<string, string> backup = contactBackupSettings[buddyname];
				contactSettings.Add(buddy, backup);
				contactBackupSettings.Remove(buddyname);
				if (backup.ContainsKey(setting))
					return backup[setting];
			}

			if (contactSettings.ContainsKey(buddy) && contactSettings[buddy].ContainsKey(setting))
				return contactSettings[buddy][setting];
			else
				return defaultValue;
		}
		public string GetCustomSetting(string setting, string defaultValue)
		{
			if (generalSettings.ContainsKey(setting))
				return generalSettings[setting];
			else
				return defaultValue;
		}
		public List<string> GetSettingList(string list)
		{
			if (customLists.ContainsKey(list))
				return customLists[list];
			else
				return null;
		}
		private string computeCrc32Hash(string inputStr)
		{
			CRC32Managed crc = new CRC32Managed();

			byte[] input = Encoding.Default.GetBytes(inputStr);
			byte[] output = crc.ComputeHash(input, 0, input.Length);
			return BitConverter.ToString(output).ToLower().Replace("-", "");
		}

		private List<IMProtocol> mProtocols;
		private Dictionary<IMProtocol, Dictionary<string, SettingDataStruct>> protocolSettings = new Dictionary<IMProtocol, Dictionary<string, SettingDataStruct>>();
		private Dictionary<IMBuddy, Dictionary<string, string>> contactSettings = new Dictionary<IMBuddy, Dictionary<string, string>>();
		private Dictionary<string, Dictionary<string, string>> contactBackupSettings = new Dictionary<string, Dictionary<string, string>>();
		private Dictionary<string, string> generalSettings = new Dictionary<string, string>();
		private Dictionary<string, List<string>> customLists = new Dictionary<string, List<string>>();
		private string xmlpath = "";
		private bool mAutoSave = true;
		private bool mLoaded;
	}
}