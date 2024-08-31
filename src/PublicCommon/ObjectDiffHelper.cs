namespace PublicCommon
    {
    public static class ObjectDiffHelper
        {
        public static Dictionary<string, object> GetChangedProperties<T>(T originalObject, T updatedObject)
            {
            var changedProperties = new Dictionary<string, object>();

            if (originalObject == null || updatedObject == null)
                {
                throw new ArgumentNullException("Both objects must be non-null.");
                }

            var originalProperties = originalObject.GetType().GetProperties();
            var updatedProperties = updatedObject.GetType().GetProperties();

            foreach (var originalProperty in originalProperties)
                {
                var updatedProperty = updatedProperties.FirstOrDefault(p => p.Name == originalProperty.Name);

                if (updatedProperty != null)
                    {
                    var originalValue = originalProperty.GetValue(originalObject);
                    var updatedValue = updatedProperty.GetValue(updatedObject);

                    // Compare values based on their types
                    if (originalValue != null && !originalValue.Equals(updatedValue))
                        {
                        changedProperties[originalProperty.Name] = updatedValue;
                        }
                    }
                }

            return changedProperties;
            }
        }
    }
