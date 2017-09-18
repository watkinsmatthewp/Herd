using Herd.Core;
using Herd.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Data.Providers
{
    public abstract class HerdKeyValuePairDataProvider : IHerdDataProvider
    {
        private static object _nexIdLock = new object();

        protected string KeyRoot { get; private set; }
        protected string KeyDelimiter { get; private set; }
        
        protected HerdKeyValuePairDataProvider(string keyRoot, string keyDelimiter)
        {
            KeyRoot = keyRoot;
            KeyDelimiter = keyDelimiter;
        }

        #region Users

        public HerdUserDataModel GetUser(long id)
        {
            return GetEntity<HerdUserDataModel>(id);
        }
        public HerdUserDataModel GetUser(string key)
        {
            return GetEntity<HerdUserDataModel>(key);
        }
        public HerdUserDataModel CreateUser(HerdUserDataModel user)
        {
            return CreateEntity(user);
        }

        public void UpdateUser(HerdUserDataModel user)
        {
            UpdateEntity(user);
        }
        public void UpdateUser(HerdUserDataModel user, string key)
        {
            UpdateEntity(user, key);
        }
        #endregion

        #region AppRegistration
        public HerdAppRegistrationDataModel GetAppRegistration(string instance) => GetEntity<HerdAppRegistrationDataModel>(instance);

        public void UpdateAppRegistration(HerdAppRegistrationDataModel registration) => UpdateEntity(registration, registration.Instance);
        #endregion

        #region Abstract overrides

        protected abstract string ReadKey(string key, string autoCreateValue = null);
        protected abstract void WriteKey(string key, string value);

        #endregion

        #region Private helpers
        private T GetEntity<T>(long id) where T : HerdDataModel
        {
            return ReadKey(BuildEntityKey<T>(id)).ParseJson<T>();
        }

        private T CreateEntity<T>(T entity) where T : HerdDataModel
        {
            lock (_nexIdLock)
            {
                var nextIdKey = BuildNextIdKey<T>();
                entity.ID = int.Parse(ReadKey(nextIdKey, "1".SerializeAsJson()));
                WriteKey(nextIdKey, (entity.ID + 1).SerializeAsJson());
            }
            UpdateEntity(entity);
            return entity;
        }

        private void UpdateEntity<T>(T entity) where T : HerdDataModel
        {
            WriteKey(BuildEntityKey<T>(entity.ID), entity.SerializeAsJson());
        }

        private string BuildEntityKey<T>(long id) where T : HerdDataModel
        {
            return string.Join(KeyDelimiter, KeyRoot, typeof(T).GetEntityName(), id);
        }

        private string BuildNextIdKey<T>()
        {
            return string.Join(KeyDelimiter, KeyRoot, typeof(T).GetEntityName(), "NextID");
        }

        private T GetEntity<T>(string id) where T : HerdDataModel
        {
            return ReadKey(BuildEntityKey<T>(id)).ParseJson<T>();
        }

        private T UpdateEntity<T>(T entity, string id) where T : HerdDataModel
        {
            WriteKey(BuildEntityKey<T>(id), entity.SerializeAsJson());
            return entity;
        }

        private string BuildEntityKey<T>(string id) where T : HerdDataModel
        {
            return string.Join(KeyDelimiter, KeyRoot, typeof(T).GetEntityName(), id);
        }
        #endregion
    }
}
