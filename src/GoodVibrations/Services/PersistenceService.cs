using System;
using System.Collections.Generic;
using System.IO;
using GoodVibrations.Persistence.ConnectionFactories;
using GoodVibrations.Interfaces.Services;
using GoodVibrations.Models;
using GoodVibrations.Persistence.DAOs;
using SQLite.Net.Interop;
using PCLStorage;
using GoodVibrations.Consts;
using GoodVibrations.Persistence.Base;
using GoodVibrations.Models.Base;
using System.Reflection;
using System.Linq;

namespace GoodVibrations.Services
{
	public class PersistenceService : IPersistenceService
	{
		private readonly ISQLitePlatform _platform;

		public PersistenceService(ISQLitePlatform platform)
		{
			_platform = platform;
			Notification = new NotificationDao();
			PhoneCall = new PhoneCallDao();
		}

		public void Initialize()
		{
            var folderPath = FileSystem.Current.LocalStorage.Path;
            GoodVibrationsConnectionFactory.Instance.Initialize(_platform, Path.Combine(folderPath, Constants.DataBase.DatabaseFileName));
					
			var db = GoodVibrationsConnectionFactory.Instance.GetConnection();
			db.BeginTransaction();

			try
			{
				db.CreateTable<PhoneCall>();
                db.CreateTable<Notification>();
			}
			catch (Exception ex)
			{
				db.Rollback();
				var stackTrace = ex.StackTrace;
			}

			db.Commit();
		}
		private readonly IDictionary<Type, object> _daoMap = new Dictionary<Type, object>();

		/// <summary>
		/// Gets the DAO.
		/// </summary>
		/// <returns>The DAO.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public GoodVibrationsBaseDao<T> GetDao<T>() where T : BaseModel, new()
		{
			if (_daoMap.ContainsKey(typeof(T)))
				return _daoMap[typeof(T)] as GoodVibrationsBaseDao<T>;

			var property = GetType()
				.GetRuntimeProperties()
				.FirstOrDefault(prop => prop.CanRead && typeof(GoodVibrationsBaseDao<T>).GetTypeInfo().IsAssignableFrom(prop.PropertyType.GetTypeInfo()));

			return (GoodVibrationsBaseDao<T>)(_daoMap[typeof(T)] = (property.GetValue(this) as GoodVibrationsBaseDao<T>));
		}

		public NotificationDao Notification
		{
			get;
			private set;
		}

		public PhoneCallDao PhoneCall
		{
			get;
			private set;
		}
	}
}