﻿using LawyerScrapper.Data;
using LawyerScrapper.Helper;
using LawyerScrapper.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LawyerScrapper.Service
{
    public class LawyerDbService : ILawyerDbService
    {
        private readonly LawyerDbContext _context;
        public LawyerDbService(LawyerDbContext context)
        {
            _context = context;
        }

        public async Task<string> InsertCourtData(List<CourtDto> courtDtos, string lg)
        {
            int createdCount = 0;
            
            foreach (var dto in courtDtos)
            {
                var lawyer = _context.Courts.AsNoTracking().Where(x => x.CourtId == dto.Id && x.DivisionId == dto.DivisionId).FirstOrDefault();

                if (lawyer == null)
                {
                    await _context.Courts.AddAsync(new Court
                    {
                        Canton = dto.Canton,
                        Codification = dto.Codification,
                        Division = dto.Division,
                        DivisionId = dto.DivisionId,
                        CourtId = dto.Id,
                        JurAnnexe = dto.JurAnnexe,
                        JurNum = dto.JurNum,
                        TypeJuridiction = dto.TypeJuridiction,
                        TypeJuridictionId = dto.TypeJuridictionId,
                        Country = lg,
                    });
                    createdCount++;
                }
                

            }
            await _context.SaveChangesAsync();
            return $"Created {createdCount} court and there already exist {courtDtos.Count - createdCount} courts";
        }

        
        public async Task<List<string>> InsertCourtCasesData(List<CourtDetailDto> courtDtos, string lg)
        {
            List<string> nameList = new List<string>();

            foreach (var courtDto in courtDtos)
            {
                var courtType = Constants.generateCourtType(courtDto.HearingGeneral);
                var courtLocation = courtDto.HearingGeneral.Remove(0, courtType.Length).Replace(",", "");
                if (courtLocation.Contains("SECTIES BURGERLIJKE EN FAMILIE"))
                {
                    courtLocation = courtLocation.Replace("SECTIES BURGERLIJKE EN FAMILIE", "");
                }
                if (courtLocation.Contains("SECTIONS CIVIL ET FAMILLE"))
                {
                    courtLocation = courtLocation.Replace("SECTIONS CIVIL ET FAMILLE", "");
                }
                if (courtLocation.Contains("SECTIE BURGERLIJKE EN FAMILIE"))
                {
                    courtLocation = courtLocation.Replace("SECTIE BURGERLIJKE EN FAMILIE", "");
                }
                if (courtLocation.Contains("SECTION CIVIL ET FAMILLE"))
                {
                    courtLocation = courtLocation.Replace("SECTION CIVIL ET FAMILLE", "");
                }

                DateTime hearingDateTime = DateTime.ParseExact($"{courtDto.HearingDate}", "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                
                foreach (var fullName in courtDto.Lawyers)
                {
                    nameList.Add(fullName);
                }

                await _context.CourtCaseAgendas.AddAsync(new CourtCaseAgenda
                {
                    CourtCaseNo = courtDto.CourtCaseNo,
                    HearingGeneral = courtDto.HearingGeneral,
                    ChamberID = courtDto.ChamberID,
                    HearingType = courtDto.HearingType,
                    HearingDateTime = hearingDateTime.ToString(),
                    CourtType = courtType,
                    CourtLocation = courtLocation,
                    LawyerName = nameList.Count > 0 ? $"[\"{string.Join("\", \"", nameList)}\"]" : "[]",
                    Country = lg,
                });

            }
            await _context.SaveChangesAsync();

            return nameList;
        }

        public async Task<string> InsertLawyerData(List<LawyerDto> lawyerDtos, string lg)
        {
            int createdCount = 0;
            int updatedCount = 0;
            foreach (var dto in lawyerDtos)
            {
                dto.Name = Utils.RemoveDutchSpecialChars(dto.Name);
                dto.SureName = Utils.RemoveDutchSpecialChars(dto.SureName);
                var lawyer = _context.Lawyers.AsNoTracking().Where(x => x.Name == dto.Name && x.SureName == dto.SureName).FirstOrDefault();

                if (lawyer == null)
                {
                    await _context.Lawyers.AddAsync(new Lawyer
                    {
                        Name = dto.Name,
                        Address = dto.Address,
                        Email = dto.Email,
                        Fax = dto.Fax,
                        Phone = dto.Phone,
                        SureName = dto.SureName,
                        Website = dto.Website,
                        Country = lg,
                    });
                    createdCount++;
                } else
                {
                    lawyer.Address = dto.Address;
                    lawyer.Email = dto.Email;
                    lawyer.Fax = dto.Fax;
                    lawyer.Phone = dto.Phone;
                    lawyer.Country = lg;
                    lawyer.Website = dto.Website;
                    _context.Entry(lawyer).State = EntityState.Modified;
                    updatedCount++;
                }
                
            }
            await _context.SaveChangesAsync();
            return $"Created {createdCount} lawyers and Updated {updatedCount} lawyers";
        }
    }
}
