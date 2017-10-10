using Herd.Core;
using Herd.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Herd.Data.Providers
{
    public abstract class KeyValuePairDataProvider : IDataProvider
    {
        private static object _nexIdLock = new object();

        protected string KeyRoot { get; private set; }
        protected string KeyDelimiter { get; private set; }

        protected KeyValuePairDataProvider(string keyRoot, string keyDelimiter)
        {
            KeyRoot = keyRoot;
            KeyDelimiter = keyDelimiter;
        }

        #region AppRegistration

        public Registration GetAppRegistration(long id) => GetEntity<Registration>(id);

        public Registration GetAppRegistration(string instance) => GetEntity<Registration>(r => r.Instance == instance);

        public Registration CreateAppRegistration(Registration appRegistration) => CreateEntity(appRegistration);

        public void UpdateAppRegistration(Registration appRegistration) => UpdateEntity(appRegistration);

        #endregion AppRegistration

        #region Users

        public UserAccount GetUser(long id) => GetEntity<UserAccount>(id);

        public UserAccount GetUser(string email) => GetEntity<UserAccount>(u => u.Email == email);

        public UserAccount CreateUser(UserAccount user) => CreateEntity(user);

        public void UpdateUser(UserAccount user) => UpdateEntity(user);

        #endregion Users

        #region Abstract overrides

        protected abstract IEnumerable<string> GetAllKeys(string rootKey);

        protected abstract string ReadKey(string key);

        protected abstract void WriteKey(string key, string value);

        #endregion Abstract overrides

        #region Private helpers

        private T GetEntity<T>(Func<T, bool> matches) where T : DataModel
        {
            return GetAllEntities<T>().FirstOrDefault(matches);
        }

        private IEnumerable<T> GetAllEntities<T>() where T : DataModel
        {
            var entityRootKey = BuildEntityRootKey<T>();
            return GetAllKeys(entityRootKey)
                .Where(key => long.TryParse(key.Split(KeyDelimiter).Last(), out _))
                .Select(key => GetEntity<T>(key));
        }

        private T GetEntity<T>(long id) where T : DataModel
        {
            return GetEntity<T>(BuildEntityKey<T>(id));
        }

        private T GetEntity<T>(string key) where T : DataModel
        {
            try
            {
                return ReadKey(key).ParseJson<T>();
            }
            catch
            {
                // Not found.
                return null;
            }
        }

        private T CreateEntity<T>(T entity) where T : DataModel
        {
            lock (_nexIdLock)
            {
                var nextIdKey = BuildNextIdKey<T>();
                string nextIdVal = null;
                try { nextIdVal = ReadKey(nextIdKey); } catch { }
                if (string.IsNullOrWhiteSpace(nextIdVal))
                {
                    nextIdVal = ((long)1).SerializeAsJson(true);
                }
                entity.ID = nextIdVal.ParseJson<long>();
                WriteKey(nextIdKey, (entity.ID + 1).SerializeAsJson(true));
            }
            UpdateEntity(entity);
            return entity;
        }

        private void UpdateEntity<T>(T entity) where T : DataModel
        {
            WriteKey(BuildEntityKey<T>(entity.ID), entity.SerializeAsJson(true));
        }

        private string BuildEntityKey<T>(long id) where T : DataModel
        {
            return string.Join(KeyDelimiter, BuildEntityRootKey<T>(), id);
        }

        private string BuildEntityRootKey<T>() where T : DataModel
        {
            return string.Join(KeyDelimiter, KeyRoot, typeof(T).GetEntityName());
        }

        private string BuildNextIdKey<T>()
        {
            return string.Join(KeyDelimiter, KeyRoot, typeof(T).GetEntityName(), "NextID");
        }

        #endregion Private helpers
    }
}