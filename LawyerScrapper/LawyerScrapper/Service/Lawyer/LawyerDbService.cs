using LawyerScrapper.Data;
using LawyerScrapper.Helper;
using LawyerScrapper.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<int> InsertCourtDetailData(List<CourtDetailDto> courtDtos)
        {
            int count = 0;
            Console.WriteLine($"==============================================>{courtDtos.Count}");
            foreach(var courtDto in courtDtos)
            {
                var courtType = courtDto.HearingGeneral.Substring(0, courtDto.HearingGeneral.IndexOf(", "));
                var courtLocation = courtDto.HearingGeneral.Substring(courtDto.HearingGeneral.IndexOf(", ") + 1);
                DateTime hearingDateTime = DateTime.ParseExact($"{courtDto.HearingDate} {courtDto.HearingHour}", "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                foreach (var fullName in courtDto.Lawyers)
                {
                    count++;
                    
                    var name = fullName.Substring(0, fullName.IndexOf(" ") < 0 ? fullName.Length : fullName.IndexOf(" "));
                    var lastName = fullName.IndexOf(" ") < 0 ? "" : fullName.Substring(fullName.IndexOf(" ") + 1);
                    await _context.CourtCaseAgendas.AddAsync(new CourtCaseAgenda
                    {
                        CourtCaseNo = courtDto.CourtCaseNo,
                        HearingGeneral = courtDto.HearingGeneral,
                        ChamberID = courtDto.ChamberID,
                        HearingType = courtDto.HearingType,
                        HearingDateTime = hearingDateTime,
                        CourtType = courtType,
                        CourtLocation = courtLocation,
                        LawyerName = name,
                        LawyerSurename = lastName,
                        Country = courtDto.Country,
                    });
                }
               
            }
            await _context.SaveChangesAsync();
            return count;
        }

        public List<string> GetDistintFullNames()
        {
            return _context.CourtCaseAgendas.Select(x => x.LawyerName + " " + x.LawyerSurename).Distinct().ToList();
        }

        public async Task<string> InsertLawyerData(List<LawyerDto> lawyerDtos)
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
                    });
                    createdCount++;
                } else
                {
                    lawyer.Address = dto.Address;
                    lawyer.Email = dto.Email;
                    lawyer.Fax = dto.Fax;
                    lawyer.Phone = dto.Phone;
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
