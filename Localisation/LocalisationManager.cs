using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit.Events;
using Zedarus.ToolKit.Settings;

namespace Zedarus.ToolKit.Localisation
{
	public class LocalisationManager
	{
		#region Properties
		private string _firstPage = null;
		private bool _ready = false;
		#endregion

		#region Controls
		public LocalisationManager() {}

		public void Init()
		{
			if (!_ready)
			{
				#if API_LOC_M2H
				EventManager.SendEvent<string>(IDs.Events.SetLanguage, Language.CurrentLanguage().ToString());
				#endif
				_ready = true;
			}
		}

		public void ChangeLanguage(LangCode language, bool notifyAllObjects)
		{
			#if API_LOC_M2H
			Language.SwitchLanguage(language.ToString());
			#endif
			EventManager.SendEvent<string>(IDs.Events.SetLanguage, language.ToString());

			if (notifyAllObjects)
			{
				LocaliseText[] localiseComponents = GameObject.FindObjectsOfType<LocaliseText>();
				foreach (LocaliseText localiseComponent in localiseComponents)
				{
					localiseComponent.Localise();
				}
			}
		}

		public string Localise(string key)
		{
			return Localise(key, FirstPage);
		}

		public string Localise(string key, string page)
		{
			#if API_LOC_M2H
			return Language.Get(key, page);
			#else
			return "no localisation api";
			#endif
		}

		public string Localise(string key, object enumerablePage)
		{
			if (enumerablePage is System.Enum)
			{
				System.Type type = enumerablePage.GetType();
				if (type != null)
				{
					return Localise(key, type.Name);
				}
				else
					return null;
			}
			else
				return null;
		}

		public string Localise(object enumerable)
		{
			if (enumerable is System.Enum)
			{
				System.Type type = enumerable.GetType();
				if (type != null)
				{
					return Localise(System.Enum.GetName(type, enumerable), type.Name);
				}
				else
					return null;
			}
			else
				return null;
		}
		#endregion

		#region Helpers
		private string FirstPage
		{
			get 
			{
				if (_firstPage == null)
				{
					#if API_LOC_M2H
					List<string> list = new List<string>(Language.Sheets.Keys);
					_firstPage = list[list.Count - 1];
					list.Clear();
					list = null;
					#else
					_firstPage = "";
					#endif
				}

				return _firstPage;
			}
		}

		public bool Ready
		{
			get { return _ready; }
		}
		#endregion

		#if UNITY_EDITOR
		private static Dictionary<string, List<string>> _data = new Dictionary<string, List<string>>();

		public static bool HasData
		{
			get { return _data.Count > 0; }
		}

		public static void UpdateData()
		{
			_data.Clear();

			#if API_LOC_M2H
			Language.LoadAvailableLanguages();
			Language.SwitchLanguage(LanguageCode.EN);

			// TODO: this should be modular for different libs
			foreach (KeyValuePair<string, Dictionary<string, string>> sheet in Language.Sheets)
			{
				List<string> phrases = new List<string>();

				foreach (KeyValuePair<string, string> phrase in sheet.Value)
				{
					phrases.Add(phrase.Key);
				}

				_data.Add(sheet.Key, phrases);
			}
			#endif
		}

		public static string[] Sheets
		{
			get
			{
				List<string> list = new List<string>(_data.Keys);
				list.Reverse();
				return list.ToArray();
			}
		}

		public static string[] GetPhrasesForSheet(string sheet)
		{
			if (_data.ContainsKey(sheet))
				return _data[sheet].ToArray();
			else
				return null;
		}
		#endif

		#region Settings
		public enum LangCode
		{
			N,//null
			AA, //Afar
			AB, //Abkhazian
			AF, //(Zuid) Afrikaans
			AM, //Amharic
			AR, //Arabic
			AR_SA, //* Arabic (Saudi Arabia)
			AR_EG, //* Arabic (Egypt)
			AR_DZ, //* Arabic (Algeria)
			AR_YE, //* Arabic (Yemen)
			AR_JO, //* Arabic (Jordan)
			AR_KW, //* Arabic (Kuwait)
			AR_BH, //* Arabic (Bahrain)
			AR_IQ, //* Arabic (Iraq)
			AR_MA, //* Arabic (Libya) 
			AR_LY, //* Arabic (Morocco)
			AR_OM, //* Arabic (Oman)
			AR_SY, //* Arabic (Syria)
			AR_LB, //* Arabic (Lebanon)
			AR_AE, //* Arabic (U.A.E.)
			AR_QA, //* Arabic (Qatar)
			AS, //Assamese
			AY, //Aymara
			AZ, //Azerbaijani
			BA, //Bashkir
			BE, //Byelorussian
			BG, //Bulgarian
			BH, //Bihari
			BI, //Bislama
			BN, //Bengali
			BO, //Tibetan
			BR, //Breton
			CA, //Catalan
			CO, //Corsican
			CS, //Czech
			CY, //Welch
			DA, //Danish
			DE, //German
			DE_AT, //* German (Austria)
			DE_LI, //* German (Liechtenstein)
			DE_CH, //* German (Switzerland)
			DE_LU, //* German (Luxembourg)
			DZ, //Bhutani
			EL, //Greek
			EN, //English
			EN_US, //* English (United States)
			EN_AU, //* English (Australia)
			EN_NZ, //* English (New Zealand)
			EN_ZA, //* English (South Africa)
			EN_CB, //* English (Caribbean)
			EN_TT, //* English (Trinidad)
			EN_GB, //* English (United Kingdom)
			EN_CA, //* English (Canada)
			EN_IE, //* English (Ireland)
			EN_JM, //* English (Jamaica)
			EN_BZ, //* English (Belize)
			EO, //Esperanto
			ES, //Spanish (Spain)
			ES_MX, //* Spanish (Mexico)
			ES_CR, //* Spanish (Costa Rica)
			ES_DO, //* Spanish (Dominican Republic)
			ES_CO, //* Spanish (Colombia)
			ES_AR, //* Spanish (Argentina)	
			ES_CL, //* Spanish (Chile)	
			ES_PY, //* Spanish (Paraguay)	
			ES_SV, //* Spanish (El Salvador)	
			ES_NI, //* Spanish (Nicaragua)	
			ES_GT, //* Spanish (Guatemala)	
			ES_PA, //* Spanish (Panama)	
			ES_VE, //* Spanish (Venezuela)	
			ES_PE, //* Spanish (Peru)
			ES_EC, //* Spanish (Ecuador)
			ES_UY, //* Spanish (Uruguay)
			ES_BO, //* Spanish (Bolivia)
			ES_HN, //* Spanish (Honduras)
			ES_PR, //* Spanish (Puerto Rico)
			ET, //Estonian
			EU, //Basque
			FA, //Persian
			FI, //Finnish
			FJ, //Fiji
			FO, //Faeroese
			FR, //French (Standard)
			FR_BE, //* French (Belgium)
			FR_CH, //* French (Switzerland)
			FR_CA, //* French (Canada)
			FR_LU, //* French (Luxembourg)
			FY, //Frisian
			GA, //Irish
			GD, //Scots Gaelic
			GL, //Galician
			GN, //Guarani
			GU, //Gujarati
			HA, //Hausa
			HI, //Hindi
			HE, //Hebrew
			HR, //Croatian
			HU, //Hungarian
			HY, //Armenian
			IA, //Interlingua
			ID, //Indonesian
			IE, //Interlingue
			IK, //Inupiak
			IN, //former Indonesian
			IS, //Icelandic
			IT, //Italian
			IT_CH, //* Italian (Switzerland)
			IU, //Inuktitut (Eskimo)
			IW, //DEPRECATED: former Hebrew
			JA, //Japanese
			JI, //DEPRECATED: former Yiddish
			JW, //Javanese
			KA, //Georgian
			KK, //Kazakh
			KL, //Greenlandic
			KM, //Cambodian
			KN, //Kannada
			KO, //Korean
			KS, //Kashmiri
			KU, //Kurdish
			KY, //Kirghiz
			LA, //Latin
			LN, //Lingala
			LO, //Laothian
			LT, //Lithuanian
			LV, //Latvian, Lettish
			MG, //Malagasy
			MI, //Maori
			MK, //Macedonian
			ML, //Malayalam
			MN, //Mongolian
			MO, //Moldavian
			MR, //Marathi
			MS, //Malay
			MT, //Maltese
			MY, //Burmese
			NA, //Nauru
			NE, //Nepali
			NL, //Dutch (Standard)
			NL_BE, //*  Dutch (Belgium)
			NO, //Norwegian
			OC, //Occitan
			OM, //(Afan) Oromo
			OR, //Oriya
			PA, //Punjabi
			PL, //Polish
			PS, //Pashto, Pushto
			PT, //Portuguese
			PT_BR, //* BRazilian PT - Only used if you manually set it
			QU, //Quechua
			RM, //Rhaeto-Romance
			RN, //Kirundi
			RO, //Romanian
			RO_MO, //* Romanian (Republic of Moldova)
			RU, //Russian
			RU_MO, //* Russian (Republic of Moldova)
			RW, //Kinyarwanda
			SA, //Sanskrit
			SD, //Sindhi
			SG, //Sangro
			SH, //Serbo-Croatian
			SI, //Singhalese
			SK, //Slovak
			SL, //Slovenian
			SM, //Samoan
			SN, //Shona
			SO, //Somali
			SQ, //Albanian
			SR, //Serbian
			SS, //Siswati
			ST, //Sesotho
			SU, //Sudanese
			SV, //Swedish
			SV_FI, //* Swedish (finland)
			SW, //Swahili
			TA, //Tamil
			TE, //Tegulu
			TG, //Tajik
			TH, //Thai
			TI, //Tigrinya
			TK, //Turkmen
			TL, //Tagalog
			TN, //Setswana
			TO, //Tonga
			TR, //Turkish
			TS, //Tsonga
			TT, //Tatar
			TW, //Twi
			UG, //Uigur
			UK, //Ukrainian
			UR, //Urdu
			UZ, //Uzbek
			VI, //Vietnamese
			VO, //Volapuk
			WO, //Wolof
			XH, //Xhosa
			YI, //Yiddish
			YO, //Yoruba
			ZA, //Zhuang
			ZH, //Chinese Simplified
			ZH_TW, //* Chinese Traditional / Chinese (Taiwan)
			ZH_HK, //* Chinese (Hong Kong SAR)
			ZH_CN, //* Chinese (PRC)
			ZH_SG, //* Chinese (Singapore)
			ZU  //Zulu
		}
		#endregion
	}
}
