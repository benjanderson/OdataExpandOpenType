namespace OdataExpandOpenType.Controllers
{
    using MongoDB.Bson;

    public abstract class ParentEntity
    {
        public ParentEntity()
        {
            this._id = ObjectId.GenerateNewId().ToString();
        }

        public string Id
        {
            get { return this._id; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    this._id = ObjectId.GenerateNewId().ToString();
                else
                    this._id = value;
            }
        }

        private string _id;

    }
}