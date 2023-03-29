using KBS.App.TaxonFinder.Models;
using KBS.App.TaxonFinder.Services;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace KBS.App.TaxonFinder.Data
{
    public class RecordDatabase
    {
        readonly SQLiteAsyncConnection database;

        public RecordDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);

            Task[] tasks = new Task[4];
            tasks[0] = database.CreateTableAsync<RecordModel>();
            tasks[1] = database.CreateTableAsync<MediaFileModel>();
            tasks[2] = database.CreateTableAsync<RegisterModel>();
            tasks[3] = database.CreateTableAsync<PositionModel>();

            Task.WaitAll(tasks);

        }
        public Task<int> Register(string deviceHash, string username)
        {
            return database.InsertOrReplaceAsync(new RegisterModel { ID = 0, DeviceHash = deviceHash, UserName = username });
        }

        public Task<int> Register(string deviceHash, string username, string firstName, string lastName)
        {
            return database.InsertOrReplaceAsync(new RegisterModel { ID = 0, DeviceHash = deviceHash, UserName = username, FirstName = firstName, LastName = lastName });
        }

        public Task<RegisterModel> GetRegister()
        {
            if (database.Table<RegisterModel>().FirstOrDefaultAsync().Result != null)
            {
                if (database.Table<RegisterModel>().FirstOrDefaultAsync().Result.DeviceHash != null)
                    return database.Table<RegisterModel>().FirstOrDefaultAsync();
                else
                    return null;
            }
            else
            {
                return null;
            }

        }

        public string GetUserName()
        {
            if (database.Table<RegisterModel>().FirstOrDefaultAsync().Result != null)
            {
                if (database.Table<RegisterModel>().FirstOrDefaultAsync().Result.DeviceHash != null)
                {
                    var reg = database.Table<RegisterModel>().FirstOrDefaultAsync();
                    return reg.Result.UserName;
                }
                else
                    return null;
            }
            else
            {
                return null;
            }

        }

        public string GetUserFullName()
        {
            if (database.Table<RegisterModel>().FirstOrDefaultAsync().Result != null)
            {
                if (database.Table<RegisterModel>().FirstOrDefaultAsync().Result.DeviceHash != null)
                {
                    var reg = database.Table<RegisterModel>().FirstOrDefaultAsync();
                    return $@"{reg.Result.FirstName} {reg.Result.LastName}";
                }
                else
                    return null;
            }
            else
            {
                return null;
            }

        }

        public Task<int> Logout()
        {
            try
            {
                database.Table<RecordModel>().DeleteAsync(i => i.LocalRecordId != null);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
            return (database.InsertOrReplaceAsync(new RegisterModel { ID = 0, DeviceHash = null }));
        }

        public Task<List<RecordModel>> GetRecordsAsync()
        {
            return database.Table<RecordModel>().OrderByDescending(p => p.IsEditable).ThenByDescending(p => p.RecordDate).ThenByDescending(p => p.CreationDate).ToListAsync();
        }

        public void SetSynced(int localRecordId)
        {
            RecordModel record = database.Table<RecordModel>().Where(p => p.LocalRecordId == localRecordId).FirstOrDefaultAsync().Result;
            record.IsEditable = false;
            record.IsSynced = true;
            UpdateRecord(record);
        }
        public Task<RecordModel> GetRecordAsync(int localRecordId)
        {
            return database.Table<RecordModel>().Where(p => p.LocalRecordId == localRecordId).FirstOrDefaultAsync();
        }
        public Task<int> SaveRecord(RecordModel recordModel)
        {
            return (database.InsertAsync(recordModel));
        }
        public Task<int> UpdateRecord(RecordModel recordModel)
        {
            return (database.UpdateAsync(recordModel));
        }
        public Task<int> DeleteRecord(RecordModel recordModel)
        {
            return (database.DeleteAsync(recordModel));
        }

        public void SetRecordDeletionDate(RecordModel recordModel)
        {
            recordModel.DeletionDate = System.DateTime.Now;
            //return (database.DeleteAsync(recordModel));
        }

        public Task<List<MediaFileModel>> GetMediaAsync(int localRecordId)
        {
            return database.Table<MediaFileModel>().Where(p => p.LocalRecordId == localRecordId).ToListAsync();
        }
        public Task<MediaFileModel> GetMediaByPathAsync(string mediaPath)
        {
            return database.Table<MediaFileModel>().Where(p => p.Path == mediaPath).FirstOrDefaultAsync();
        }
        public Task<int> SaveMedia(MediaFileModel mediaFileModel)
        {
            return (database.InsertAsync(mediaFileModel));
        }
        public Task<int> UpdateMedia(MediaFileModel mediaFileModel)
        {
            return (database.UpdateAsync(mediaFileModel));
        }
        public Task<int> DeleteMedia(MediaFileModel mediaFileModel)
        {
            return (database.DeleteAsync(mediaFileModel));
        }

        public Task<int> SavePosition(PositionModel positionModel)
        {
            return (database.InsertAsync(positionModel));
        }
        public Task<int> DeletePosition(PositionModel positionModel)
        {
            return (database.DeleteAsync(positionModel));

        }
        public Task<List<PositionModel>> GetPositionAsync(int localRecordId)
        {
            return database.Table<PositionModel>().Where(p => p.LocalRecordId == localRecordId).ToListAsync();
        }
        public Task<List<PositionModel>> GetAllPositionsAsync()
        {
            return database.Table<PositionModel>().ToListAsync();
        }

        internal int? UpdateFromSyncList(List<AdviceJsonItemSync> adviceList)
        {
            try
            {
                int cnt = 0;
                //database.InsertOrReplaceAsync((RecordModel)advice);
                //@TODO: delete all and rebuild??
                foreach (AdviceJsonItemSync ajis in adviceList)
                {
                    string identifierString = ajis.Identifier.ToString();
                    RecordModel rm = database.Table<RecordModel>().Where(p => (p.Identifier == identifierString)).FirstOrDefaultAsync().Result;
                    if (rm != null)
                    {
                        rm.GlobalAdviceId = ajis.GlobalAdviceId;
                        rm.Identifier = ajis.Identifier.ToString();
                        rm.TaxonId = ajis.TaxonId;
                        rm.TaxonGuid = ajis.TaxonGuid.ToString();
                        rm.RecordDate = (DateTime)ajis.AdviceDate;
                        rm.TotalCount = ajis.AdviceCount.HasValue ? ajis.AdviceCount.Value : 0;
                        rm.HabitatName = ajis.AdviceCity;
                        rm.MaleCount = ajis.MaleCount;
                        rm.FemaleCount = ajis.FemaleCount;
                        rm.StateEgg = ajis.StateEgg;
                        rm.StateLarva = ajis.StateLarva;
                        rm.StateImago = ajis.StateImago;
                        rm.DiagnosisTypeId = ajis.DiagnosisTypeId;
                        rm.StateNymph = ajis.StateNymph;
                        rm.StatePupa = ajis.StatePupa;
                        rm.StateDead = ajis.StateDead;
                        rm.HabitatDescription = ajis.Comment;
                        rm.HabitatDescriptionForEvent = ajis.HabitatDescriptionForEvent;
                        rm.ReportedByName = ajis.ReportedByName;
                        rm.LastEdit = TruncateDateTime(ajis.LastEditDate, TimeSpan.FromSeconds(1));
                        rm.DeletionDate = ajis.DeletionDate;
                        rm.ImageCopyright = ajis.ImageCopyright;
                        rm.ImageLegend = ajis.ImageLegend;
                        rm.Latitude = Double.Parse(ajis.Lat.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture);
                        rm.Longitude = Double.Parse(ajis.Lon.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture);
                        rm.AccuracyTypeId = ajis.AccuracyTypeId;
                        rm.LocalityTemplateId = ajis.LocalityTemplateId;
                        //rm.IsSynced = ajis.IsSynced;
                        rm.IsSynced = true;
                        rm.UserName = ajis.UserName;
                        rm.ApprovalStateId = ajis.ApprovalStateId;
                        rm.IsEditable = true;
                        if (ajis.ApprovalStateId > 1)
                        {
                            rm.IsEditable = false;
                        }
                        UpdateRecord(rm);
                        cnt++;

                    }
                    else
                    {
                        rm = new RecordModel();
                        //rm.LocalRecordId = ajis.MobileAdviceId != null ? (int)ajis.MobileAdviceId : -1;
                        if (ajis.GlobalAdviceId != null)
                        {
                            rm.LocalRecordId = (int)ajis.GlobalAdviceId;
                        }
                        rm.GlobalAdviceId = ajis.GlobalAdviceId;
                        rm.Identifier = ajis.Identifier.ToString();
                        rm.TaxonId = ajis.TaxonId;
                        rm.TaxonGuid = ajis.TaxonGuid.ToString();
                        rm.RecordDate = (DateTime)ajis.AdviceDate;
                        rm.FemaleCount = (int?)ajis.FemaleCount;
                        rm.MaleCount = (int?)ajis.MaleCount;
                        rm.TotalCount = (int?)ajis.AdviceCount;
                        rm.HabitatName = ajis.AdviceCity;
                        rm.StateEgg = ajis.StateEgg;
                        rm.StateLarva = ajis.StateLarva;
                        rm.StateImago = ajis.StateImago;
                        rm.DiagnosisTypeId = ajis.DiagnosisTypeId;
                        rm.StateNymph = ajis.StateNymph;
                        rm.StatePupa = ajis.StatePupa;
                        rm.StateDead = ajis.StateDead;
                        rm.HabitatDescription = ajis.Comment;
                        rm.HabitatDescriptionForEvent = ajis.HabitatDescriptionForEvent;
                        rm.ReportedByName = ajis.ReportedByName;
                        rm.LastEdit = TruncateDateTime(ajis.LastEditDate, TimeSpan.FromSeconds(1));
                        rm.DeletionDate = ajis.DeletionDate;
                        rm.ImageCopyright = ajis.ImageCopyright;
                        rm.ImageLegend = ajis.ImageLegend;
                        rm.Latitude = Double.Parse(ajis.Lat.Replace(',','.'), NumberStyles.Any, CultureInfo.InvariantCulture);
                        rm.Longitude = Double.Parse(ajis.Lon.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture);
                        rm.AccuracyTypeId = ajis.AccuracyTypeId;
                        rm.LocalityTemplateId = ajis.LocalityTemplateId;
                        rm.IsSynced = true;
                        rm.UserName = ajis.UserName;

                        rm.ApprovalStateId = ajis.ApprovalStateId;
                        rm.IsEditable = true;
                        if (ajis.ApprovalStateId > 1)
                        {
                            rm.IsEditable = false;
                        }

                        SaveRecord(rm);
                        cnt++;
                    }
                    if (ajis.Images.Count > 0)
                    {
                        UpdateMediaFromSyncList(ajis.Images, rm.LocalRecordId);
                    }

                }
                return cnt;
            }
            catch (Exception ex)
            {
                var dbg = ex;
                Trace.WriteLine(ex.Message);
                return null;
            }
        }

        public async void UpdateMediaFromSyncList(List<AdviceImageJsonItem> imageList, int recordModelId)
        {
            var fileHelper = DependencyService.Get<IFileHelper>();

            List<MediaFileModel> mfmList = await GetMediaAsync(recordModelId);
            List<MediaFileModel> mfm_backup = mfmList;
            foreach (MediaFileModel mfmItem in mfmList)
            {
                await DeleteMedia(mfmItem);
            }

            try
            {
                foreach (AdviceImageJsonItem imageItem in imageList)
                {
                    try
                    {
                        if (imageItem != null)
                        {
                            if (fileHelper.FileExists(fileHelper.GetLocalAppPath(imageItem.ImageName)))
                            {
                                //string img_tbu_FullPath = Path.Combine(imgSavePath_general_dev, img_in.ImageName);
                                byte[] img_tbu_imageArray = System.IO.File.ReadAllBytes(fileHelper.GetLocalAppPath(imageItem.ImageName));
                                string img_tbu_base64 = Convert.ToBase64String(img_tbu_imageArray);

                                if (img_tbu_base64 != imageItem.ImageBase64)
                                {
                                    fileHelper.DeleteFile(fileHelper.GetLocalAppPath(imageItem.ImageName));
                                    byte[] imageBytes = fileHelper.GetBytesFromBase64String(imageItem.ImageBase64);
                                    fileHelper.CopyFileToLocal(imageBytes, fileHelper.GetLocalAppPath(imageItem.ImageName));
                                    //var copyMedia = fileHelper.CopyFileToApp(fileHelper.GetLocalFilePath(imageItem.ImageName));
                                    //"/storage/emulated/0/Android/data/com.kbs.idoapp/files/Pictures/IMG_20220429_155020.jpg"
                                    MediaFileModel media = new MediaFileModel(fileHelper.GetLocalAppPath(imageItem.ImageName), recordModelId, MediaFileModel.MediaType.Image);
                                    await SaveMedia(media);
                                }
                                else
                                {
                                    MediaFileModel media = new MediaFileModel(fileHelper.GetLocalAppPath(imageItem.ImageName), recordModelId, MediaFileModel.MediaType.Image);
                                    await SaveMedia(media);
                                }
                            }
                            else
                            {
                                byte[] imageBytes = fileHelper.GetBytesFromBase64String(imageItem.ImageBase64);
                                fileHelper.CopyFileToLocal(imageBytes, fileHelper.GetLocalAppPath(imageItem.ImageName));
                                //var copyMedia = fileHelper.CopyFileToApp(fileHelper.GetLocalFilePath(imageItem.ImageName));
                                MediaFileModel media = new MediaFileModel(fileHelper.GetLocalAppPath(imageItem.ImageName), recordModelId, MediaFileModel.MediaType.Image);
                                await SaveMedia(media);
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                //Restore
                List<MediaFileModel> mfmList2 = await GetMediaAsync(recordModelId);
                foreach (MediaFileModel mfmItem in mfmList)
                {
                    await DeleteMedia(mfmItem);
                }

                foreach (MediaFileModel mfmItemRestore in mfm_backup)
                {
                    await SaveMedia(mfmItemRestore);
                }


            }


        }


        public static DateTime? TruncateDateTime(DateTime? dateTime, TimeSpan timeSpan)
        {
            if (dateTime != null)
            {
                if (timeSpan == TimeSpan.Zero) return dateTime; // Or could throw an ArgumentException
                if (dateTime == DateTime.MinValue || dateTime == DateTime.MaxValue) return dateTime; // do not modify "guard" values
                return ((DateTime)dateTime).AddTicks(-(((DateTime)dateTime).Ticks % timeSpan.Ticks));
            }
            return null;
        }
    }
}
