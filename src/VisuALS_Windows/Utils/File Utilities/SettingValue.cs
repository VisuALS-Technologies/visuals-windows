namespace VisuALS_WPF_App
{
    class SettingValue<T>
    {
        private T cachedValue;
        public T Value
        {
            get { return cachedValue; }
            set
            {
                cachedValue = value;
                SettingsManager.Set(relativePath, key, value);
            }
        }

        private string relativePath;
        private string key;
        private T initialValue;

        public SettingValue(string _relativePath, string _key, T _initialValue)
        {
            relativePath = _relativePath;
            key = _key;
            initialValue = _initialValue;
        }

        public static implicit operator T(SettingValue<T> sv)
        {
            return sv.Value;
        }
    }
}
