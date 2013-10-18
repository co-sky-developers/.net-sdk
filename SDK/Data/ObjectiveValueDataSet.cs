using System.Collections.Generic;


namespace NFleetSDK.Data
{
    public class ObjectiveValueDataSet : IResponseData
    {
        public int VersionNumber { get; set; }
        public List<ObjectiveValueData> Items { get; set; }
        public List<Link> Meta { get; set; }

        public ObjectiveValueDataSet()
        {
            Items = new List<ObjectiveValueData>();
            Meta = new List<Link>();
        }
    }
}
