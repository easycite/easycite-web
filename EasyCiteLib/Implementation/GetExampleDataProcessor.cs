﻿using System;
using System.Threading.Tasks;
using EasyCiteLib.DataModel;
using EasyCiteLib.Interface;

namespace EasyCiteLib.Implementation
{
    public class GetExampleDataProcessor : IGetExampleDataProcessor
    {
        public async Task<ExampleData> Get(int id)
        {
            var results = new ExampleData
            {
                FirstName = "Andrew",
                LastName = "Schmid",
                Id = id,
                Guid = Guid.NewGuid()
            };

            return results;
        }
    }
}