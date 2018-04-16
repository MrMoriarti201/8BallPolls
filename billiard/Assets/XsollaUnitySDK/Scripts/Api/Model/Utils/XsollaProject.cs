using SimpleJSON;

namespace Xsolla 
{
	public class XsollaProject : IParseble
	{

		public long id{get; private set;}// "id":14244,
		public string name{get; private set;}// "name":"TestCreateProject",
		public string nameEn{get; private set;}// "nameEn":"TestCreateProject",
		public string virtualCurrencyName{get; private set;}// "virtualCurrencyName":"virt. currency",
		public long merchantId{get; private set;}// "merchantId":2340,
		public bool isDiscrete{get; private set;}// "isDiscrete":false,
		public string projectUrl{get; private set;}// "projectUrl":"xsolla.com",
		public string returnUrl{get; private set;}// "returnUrl":"https:\/\/secure.xsolla.com?v1=user_1&v2=John+Smith",
		public bool isKeepUsers{get; private set;}// "isKeepUsers":false,
		public int recurringPackageCount{get; private set;}// "recurringPackageCount":0,
		public string eula{get; private set;}// "eula":"http:\/\/xsolla.com\/termsandconditions\/?lang=en&ca=2340",
		public bool canRepeatPayment{get; private set;}// "canRepeatPayment":true

		public IParseble Parse (JSONNode projectNode)
		{
			id = projectNode ["id"].AsInt;
			name = projectNode ["name"];
			nameEn = projectNode ["nameEn"];
			virtualCurrencyName = projectNode ["virtualCurrencyName"];
			merchantId = projectNode ["merchantId"].AsInt;
			isDiscrete = projectNode ["isDiscrete"].AsBool;
			projectUrl = projectNode ["projectUrl"];
			returnUrl = projectNode ["returnUrl"];
			isKeepUsers = projectNode ["isKeepUsers"].AsBool;
			recurringPackageCount = projectNode ["recurringPackageCount"].AsInt;
			eula = projectNode ["eula"];
			canRepeatPayment = projectNode ["canRepeatPayment"].AsBool;
			return this;
		}

		public override string ToString ()
		{
			return string.Format ("[XsollaProject: id={0}, name={1}, nameEn={2}, virtualCurrencyName={3}, merchantId={4}, isDiscrete={5}, projectUrl={6}, returnUrl={7}, isKeepUsers={8}, recurringPackageCount={9}, eula={10}, canRepeatPayment={11}]", id, name, nameEn, virtualCurrencyName, merchantId, isDiscrete, projectUrl, returnUrl, isKeepUsers, recurringPackageCount, eula, canRepeatPayment);
		}
	}
}
