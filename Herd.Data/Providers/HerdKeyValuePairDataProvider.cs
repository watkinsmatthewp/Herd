using Herd.Core;
using Herd.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public HerdUserDataModel GetUser(string email, string instance)
        {
            return GetEntity<HerdUserDataModel>(u => u.Email == email && u.MastodonInstanceHost == instance);
        }

        public HerdUserDataModel CreateUser(HerdUserDataModel user)
        {
            return CreateEntity(user);
        }

        public void UpdateUser(HerdUserDataModel user)
        {
            UpdateEntity(user);
        }

        #endregion

        #region AppRegistration

        public HerdAppRegistrationDataModel GetAppRegistration(long id)
        {
            return GetEntity<HerdAppRegistrationDataModel>(id);
        }

        public HerdAppRegistrationDataModel GetAppRegistration(string instance)
        {
            return GetEntity<HerdAppRegistrationDataModel>(r => r.Instance == instance);
        }

        public HerdAppRegistrationDataModel CreateAppRegistration(HerdAppRegistrationDataModel appRegistration)
        {
            return CreateEntity(appRegistration);
        }

        public void UpdateAppRegistration(HerdAppRegistrationDataModel appRegistration)
        {
            UpdateEntity(appRegistration);
        }

        #endregion

        #region Abstract overrides

        protected abstract IEnumerable<string> GetAllKeys(string rootKey);
        protected abstract string ReadKey(string key);
        protected abstract void WriteKey(string key, string value);

        #endregion

        #region Private helpers

        private T GetEntity<T>(Func<T, bool> matches) where T : HerdDataModel
        {
            return GetAllEntities<T>().FirstOrDefault(matches);
        }

        private IEnumerable<T> GetAllEntities<T>() where T : HerdDataModel
        {
            var entityRootKey = BuildEntityRootKey<T>();
            return GetAllKeys(entityRootKey)
                .Select(key => GetEntity<T>(key));
        }

        private T GetEntity<T>(long id) where T : HerdDataModel
        {
            return GetEntity<T>(BuildEntityKey<T>(id));
        }

        private T GetEntity<T>(string key) where T : HerdDataModel
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

        private T CreateEntity<T>(T entity) where T : HerdDataModel
        {
            lock (_nexIdLock)
            {
                var nextIdKey = BuildNextIdKey<T>();
                string nextIdVal = null;
                try { nextIdVal = ReadKey(nextIdKey); } catch { }
                if (string.IsNullOrWhiteSpace(nextIdVal))
                {
                    nextIdVal = "1";
                }
                entity.ID = int.Parse(nextIdVal);
                WriteKey(nextIdKey, (entity.ID + 1).SerializeAsJson(true));
            }
            UpdateEntity(entity);
            return entity;
        }

        private void UpdateEntity<T>(T entity) where T : HerdDataModel
        {
            WriteKey(BuildEntityKey<T>(entity.ID), entity.SerializeAsJson(true));
        }

        private string BuildEntityKey<T>(long id) where T : HerdDataModel
        {
            return string.Join(KeyDelimiter, BuildEntityRootKey<T>(), id);
        }

        private string BuildEntityRootKey<T>() where T : HerdDataModel
        {
            return string.Join(KeyDelimiter, KeyRoot, typeof(T).GetEntityName());
        }

        private string BuildNextIdKey<T>()
        {
            return string.Join(KeyDelimiter, KeyRoot, typeof(T).GetEntityName(), "NextID");
        }

        #endregion
    }
}
