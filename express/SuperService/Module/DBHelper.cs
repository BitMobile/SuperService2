using BitMobile.ClientModel3;
using BitMobile.DbEngine;
using System;
using System.Collections;
using Test.Document;
using Database = BitMobile.ClientModel3.Database;

namespace Test
{
    /// <summary>
    ///     Обеспечивает работу с базой данных приложения
    /// </summary>
    /// <remarks>
    /// </remarks>
    public static partial class DBHelper
    {
        private static Database _db;

        public static string LastError => _db.LastError;
        public static DateTime LastSyncTime => _db.LastSyncTime;
        public static bool SuccessSync => _db.SuccessSync;

        public static void Init()
        {
            _db = new Database();
            if (_db.Exists) return;

            DConsole.WriteLine("Creating DB");
            _db.CreateFromModel();
        }

        public static void SaveEntity(DbEntity entity, bool doSync = true)
        {
            entity.Save();
            _db.Commit();
            if (doSync)
                SyncAsync();
        }
        public static EventHistory CreateHistory(Event @event)
        {
            return new EventHistory
            {
                Author = Settings.UserDetailedInfo.Id,
                DeletionMark = false,
                Date = DateTime.Now,
                Event = @event.Id,
                Id = DbRef.CreateInstance("Document_EventHistory", Guid.NewGuid()),
                Status = @event.Status,
                UserMA = @event.UserMA
            };
        }

        public static void SaveEntities(IEnumerable entities, bool doSync = true)
        {
            foreach (DbEntity entity in entities)
            {
                entity.Save();
            }
            _db.Commit();
            if (doSync)
                SyncAsync();
        }

        public static void DeleteByRef(DbRef @ref, bool doSync = true)
        {
            _db.Delete(@ref);
            _db.Commit();
            if (doSync)
                SyncAsync();
        }

        public static object LoadEntity(string id)
        {
            return DbRef.FromString(id).GetObject();
        }

        public static void FullSyncAsync(ResultEventHandler<bool> resultEventHandler = null)
        {
            if (_db.SyncIsActive)
            {
#if DEBUG
                DConsole.WriteLine($"---------------{Environment.NewLine}Синхронизация не запущена," +
                                   $" происходит другая синхронизация." +
                                   $"{Environment.NewLine}Class {nameof(DBHelper)} method {nameof(FullSyncAsync)}" +
                                   $"{Environment.NewLine}---------------");
#endif
                return;
            }

#if DEBUG
            DConsole.WriteLine($"---------------{Environment.NewLine}Начинаю полную синхронизацию." +
                               $"{Environment.NewLine}Class {nameof(DBHelper)} method {nameof(FullSyncAsync)}" +
                               $"{Environment.NewLine}---------------");
#endif

            try
            {
                Utils.TraceMessage($"Sync(from Settings) login: {Settings.User} password: {Settings.Password}");
                _db.PerformFullSyncAsync(Settings.Server, Settings.User, Settings.Password, Settings.DefaultSyncTimeOut,
                    SyncHandler + resultEventHandler,
                    "Full");
            }
            catch (Exception)
            {
                SyncHandler("Full", new ResultEventArgs<bool>(false));
            }
        }

        public static void SyncAsync(ResultEventHandler<bool> resultEventHandler = null)
        {
            if (_db.SyncIsActive)
            {
#if DEBUG
                DConsole.WriteLine($"---------------{Environment.NewLine}Синхронизация не запущена," +
                                   $" происходит другая синхронизация." +
                                   $"{Environment.NewLine}Class {nameof(DBHelper)} method {nameof(SyncAsync)}" +
                                   $"{Environment.NewLine}---------------");
#endif
                return;
            }

#if DEBUG
            DConsole.WriteLine($"---------------{Environment.NewLine}Начинаю частичную синхронизацию." +
                               $"{Environment.NewLine}Class {nameof(DBHelper)} method {nameof(SyncAsync)}" +
                               $"{Environment.NewLine}---------------");
#endif

            try
            {
                _db.PerformSyncAsync(Settings.Server, Settings.User, Settings.Password, Settings.DefaultSyncTimeOut,
                    SyncHandler + resultEventHandler,
                    "Partial");
            }
            catch (Exception)
            {
                SyncHandler("Partial", new ResultEventArgs<bool>(false));
            }
        }

        public static void Sync(ResultEventHandler<bool> resultEventHandler = null)
        {
            if (_db.SyncIsActive)
            {
#if DEBUG
                DConsole.WriteLine($"---------------{Environment.NewLine}Синхронизация не запущена," +
                                   $" происходит другая синхронизация." +
                                   $"{Environment.NewLine}Class {nameof(DBHelper)} method {nameof(Sync)}" +
                                   $"{Environment.NewLine}---------------");
#endif
                return;
            }

#if DEBUG
            DConsole.WriteLine($"---------------{Environment.NewLine}Начинаю частичную синхронизацию." +
                               $"{Environment.NewLine}Class {nameof(DBHelper)} method {nameof(Sync)}" +
                               $"{Environment.NewLine}---------------");
#endif

            try
            {
                _db.PerformSync(Settings.Server, Settings.User, Settings.Password, Settings.DefaultSyncTimeOut,
                    SyncHandler + resultEventHandler,
                    "Partial");
            }
            catch (Exception)
            {
                SyncHandler("Partial", new ResultEventArgs<bool>(false));
            }
        }

        private static void SyncHandler(object state, ResultEventArgs<bool> resultEventArgs)
        {
            if (state.Equals("Full"))
            {
                Toast.MakeToast(Translator.Translate(resultEventArgs.Result ? "sync_success" : "sync_fail"));
            }
            else
            {
#if DEBUG
                DConsole.WriteLine($"---------------{Environment.NewLine}"
                                   + Translator.Translate(resultEventArgs.Result ? "sync_success" : "sync_fail"));
                DConsole.WriteLine($"Последняя ошибка: {LastError}");
                DConsole.WriteLine($"Результат синхронизации в callback {resultEventArgs.Result}" +
                                   $"{Environment.NewLine}{nameof(SuccessSync)}: {SuccessSync}" +
                                   $"{Environment.NewLine}---------------");
#endif
            }
            if (!resultEventArgs.Result)
            {
#if DEBUG
                DConsole.WriteLine(Parameters.Splitter);
                DConsole.WriteLine($"Новые данные не пришли," +
                                   $"настройки не обновляем" +
                                   $" {nameof(resultEventArgs.Result)} = {resultEventArgs.Result}" +
                                   $"{Environment.NewLine}Last Error {_db.LastError}");
                DConsole.WriteLine(Parameters.Splitter);
#endif
                return;
            }
#if DEBUG
            DConsole.WriteLine(Parameters.Splitter);
            DConsole.WriteLine("Пришли новые настройки. Обновляем их");
            DConsole.WriteLine(Parameters.Splitter);
#endif
            Application.InvokeOnMainThread(() => GpsTracking.Stop());

            Settings.Init();

            Application.InvokeOnMainThread(() => GpsTracking.Start());

            DynamicScreenRefreshService.RefreshScreen();
        }

        public static void FullSync(ResultEventHandler<bool> resultEventHandler = null)
        {
            if (_db.SyncIsActive)
            {
#if DEBUG
                DConsole.WriteLine($"---------------{Environment.NewLine}Синхронизация не запущена," +
                                   $" происходит другая синхронизация." +
                                   $"{Environment.NewLine}Class {nameof(DBHelper)} method {nameof(FullSync)}" +
                                   $"{Environment.NewLine}---------------");
#endif
                return;
            }

#if DEBUG
            DConsole.WriteLine($"---------------{Environment.NewLine}Начинаю полную синхронизацию." +
                               $"{Environment.NewLine}Class {nameof(DBHelper)} method {nameof(FullSync)}" +
                               $"{Environment.NewLine}---------------");
#endif

            try
            {
                _db.PerformFullSync(Settings.Server, Settings.User, Settings.Password, Settings.DefaultSyncTimeOut,
                    SyncHandler + resultEventHandler,
                    "Full");
            }
            catch (Exception)
            {
                SyncHandler("Full", new ResultEventArgs<bool>(false));
            }
        }
    }
}