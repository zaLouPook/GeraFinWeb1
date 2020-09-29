﻿using System;
using System.Linq;
using System.Threading;
using GeraFin.DAL.DataAccess;
using GeraFin.InterFaces.Factory;

namespace GeraFin.Services
{
    public class NumberSequence : INumberSequence
    {
        private readonly ApplicationDbContext _context;

        public NumberSequence(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GetNumberSequence(string module)
        {
            string result = "";
            try
            {
                int counter = 0;

                Models.ViewModels.GeraFinWeb.NumberSequence numberSequence = _context.NumberSequence
                    .Where(x => x.Module.Equals(module))
                    .FirstOrDefault();

                if (numberSequence == null)
                {
                    numberSequence = new Models.ViewModels.GeraFinWeb.NumberSequence
                    {
                        Module = module
                    };
                    Interlocked.Increment(ref counter);
                    numberSequence.LastNumber = counter;
                    numberSequence.NumberSequenceName = module;
                    numberSequence.Prefix = module;

                    _context.Add(numberSequence);
                    _context.SaveChanges();
                }
                else
                {
                    counter = numberSequence.LastNumber;

                    Interlocked.Increment(ref counter);
                    numberSequence.LastNumber = counter;

                    _context.Update(numberSequence);
                    _context.SaveChanges();
                }
                result = counter.ToString().PadLeft(5, '0') + "#" + numberSequence.Prefix;
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }
    }
}
