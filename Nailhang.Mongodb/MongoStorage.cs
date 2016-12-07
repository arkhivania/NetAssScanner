﻿using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Nailhang.IndexBase.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.Mongodb
{
    class MongoStorage : IModulesStorage
    {
        readonly IMongoCollection<ModuleEntity> modules;

        readonly string dbName;

        public MongoStorage(MongoConnection mongoConnection)
        {
            var connectionString = mongoConnection.ConnectionString;
            this.dbName = mongoConnection.DbName;

            var mongoParam = Environment
                .GetCommandLineArgs()
                .FirstOrDefault(w => w.ToLower().StartsWith("-mongo:"));
            if(mongoParam != null)
                connectionString = mongoParam.Substring("-mongo:".Length);

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(dbName);
            this.modules = database.GetCollection<ModuleEntity>("modules");
        }

        public void StoreModule(IndexBase.Module module)
        {
            var mentity = new ModuleEntity
            {
                ModuleHeader = JsonConvert.SerializeObject(module),
                Id = module.FullName.GenerateGuid(),
                FullName = module.FullName
            };


            var filter = Builders<ModuleEntity>.Filter.Where(w => w.Id == mentity.Id);
            var replaceResult = modules.ReplaceOne(filter, mentity, new UpdateOptions { IsUpsert = true });            
        }

        public IEnumerable<IndexBase.Module> GetModules()
        {   
            foreach (var moduleEntity in modules.AsQueryable<ModuleEntity>())
                yield return ToModule(moduleEntity.ModuleHeader);
        }

        IndexBase.Module ToModule(string header)
        {
            return JsonConvert.DeserializeObject<IndexBase.Module>(header);
        }

        public void DropModules(string namespaceStartsWith)
        {
            if (string.IsNullOrEmpty(namespaceStartsWith))
                modules.DeleteMany(new MongoDB.Bson.BsonDocument());
            else
                modules.DeleteMany(Builders<ModuleEntity>.Filter
                    .Where(w => w
                        .FullName
                        .StartsWith(namespaceStartsWith)));
        }
    }
}
