using System.Reflection;

namespace OrganizationManagementApplication.Services
{
    /// <summary>
    /// Service class providing helper methods for working with object properties using reflection.
    /// </summary>
    public class ReflectionHelperService
    {
        /// <summary>
        /// Sets the value of a specified property on an object using reflection.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">The object on which to set the property value.</param>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="value">The value to set on the property.</param>
        public void SetProperty<T>(T obj, string propertyName, object value)
        {
            var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (property != null && property.CanWrite)
            {
                property.SetValue(obj, value);
            }
        }

        /// <summary>
        /// Gets the value of a specified property from an object using reflection.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">The object from which to get the property value.</param>
        /// <param name="propertyName">The name of the property to get.</param>
        /// <returns>The value of the property, or null if the property does not exist.</returns>
        public object GetProperty<T>(T obj, string propertyName)
        {
            var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            return property?.GetValue(obj);
        }

        /// <summary>
        /// Checks if a specified property exists on a type using reflection.
        /// </summary>
        /// <typeparam name="T">The type to check for the property.</typeparam>
        /// <param name="propertyName">The name of the property to check.</param>
        /// <returns>True if the property exists, otherwise false.</returns>
        public bool PropertyExists<T>(string propertyName)
        {
            var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            return property != null;
        }
    }
}
