using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SagicorNow.Core
{
    public class FromJsonAttribute : CustomModelBinderAttribute
    {
        private static readonly JavaScriptSerializer Serializer = new JavaScriptSerializer();

        public override IModelBinder GetBinder()
        {
            return new JsonModelBinder();
        }

        private class JsonModelBinder : IModelBinder
        {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                var stringified = controllerContext.HttpContext.Request[bindingContext.ModelName];
                if (string.IsNullOrEmpty(stringified))
                    return null;
                return Serializer.Deserialize(stringified, bindingContext.ModelType);
                
            }
        }
    }
}