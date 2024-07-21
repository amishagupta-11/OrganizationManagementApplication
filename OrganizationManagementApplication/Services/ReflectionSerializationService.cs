using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Reflection;
using OrganizationManagementApplication.Models;
using System.Xml.Serialization;

namespace OrganizationManagementApplication.Services
{
    public class ReflectionSerializationService
    {
        /// <summary>
        /// Serialize the Employee object to JSON using reflection
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public string SerializeToJson(Employee employee)
        {
            var jsonObject = new JObject();

            // Get all public properties of the Employee class
            foreach (var property in typeof(Employee).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var value = property.GetValue(employee);
                var jsonPropertyAttribute = property.GetCustomAttribute<JsonPropertyAttribute>();

                var propertyName = jsonPropertyAttribute?.PropertyName ?? property.Name;


                jsonObject[propertyName] = JToken.FromObject(value);
            }

            return jsonObject.ToString(Formatting.Indented);
        }

        /// <summary>
        /// Deserialize JSON to Employee object using reflection
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public Employee DeserializeFromJson(string json)
        {
            var jsonObject = JObject.Parse(json);
            var employee = Activator.CreateInstance<Employee>();

            // loop to fetch all public properties of the Employee class
            foreach (var property in typeof(Employee).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var jsonPropertyAttribute = property.GetCustomAttribute<JsonPropertyAttribute>();
                var propertyName = jsonPropertyAttribute?.PropertyName ?? property.Name;

                if (jsonObject.TryGetValue(propertyName, StringComparison.OrdinalIgnoreCase, out var token))
                {
                    var value = token.ToObject(property.PropertyType);
                    property.SetValue(employee, value);
                }
            }

            return employee;
        }

        /// <summary>
        /// Serialize the Employee object to XML using XmlSerializer
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public string SerializeToXml(Employee employee)
        {
            ArgumentNullException.ThrowIfNull(employee);

            XmlSerializer serializer = new (typeof(Employee));
            StringWriter writer = new ();
            serializer.Serialize(writer, employee);
            return writer.ToString();
        }

        /// <summary>
        /// Deserialize XML to Employee object using XmlSerializer
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public Employee DeserializeFromXml(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                throw new ArgumentNullException(nameof(xml));
            }

            XmlSerializer serializer = new (typeof(Employee));
            using StringReader reader = new(xml);
            return (Employee)serializer.Deserialize(reader);
        }
    }
}
