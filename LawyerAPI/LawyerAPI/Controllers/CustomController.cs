using LawyerAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LawyerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomController : ControllerBase
    {
        private readonly LawyerDbContext _context;

        public CustomController(LawyerDbContext context)
        {
            _context = context;
        }

        // GET: api/Custom/CourtCaseByDateAndCourtName/2022-10-03/democourtname
        [HttpGet("CourtCaseByDateAndCourtName/{date}/{courtname}")]
        public async Task<ActionResult<IEnumerable<CourtCaseAgenda>>> GetCourtCaseByDateAndCourtName(string date, string courtname)
        {
            return await _context.CourtCaseAgenda.AsNoTracking().Where(x => x.HearingDateTime == date && (x.CourtType!.Contains(courtname) || x.CourtLocation!.Contains(courtname))).ToListAsync();
        }

        // GET: api/Custom/CourtCaseBydateAndEmail/2022-10-03/demoemail
        [HttpGet("CourtCaseByDateAndEmail/{date}/{lawyeremail}")]
        public async Task<ActionResult<IEnumerable<CourtCaseAgenda>>> GetCourtCaseByDateAndEmail(string date, string lawyeremail)
        {
            var lawyers = (from b in _context.CourtCaseAgenda
                          join a in _context.Lawyers
                          on new { LawyerName = b.LawyerName, LawyerSurename = b.LawyerSurename } 
                          equals new { LawyerName = a.Name, LawyerSurename = a.SureName }
                          where (a.Email == lawyeremail) &&
                                (b.HearingDateTime == date)
                          select b).Distinct();

            if (lawyers == null)
            {
                return NotFound();
            }
            return await lawyers.ToListAsync();
        }

        // GET: api/Custom/LawyersByCourtCaseId/234
        [HttpGet("LawyersByCourtCaseId/{courtcaseid}")]
        public async Task<ActionResult<List<Lawyer>>> GetLawyersByCourtCaseId(string courtcaseid)
        {
            var lawyers = (from a in _context.Lawyers
                          join b in _context.CourtCaseAgenda
                          on new { Name = a.Name, Surename = a.SureName }
                          equals new { Name = b.LawyerName, Surename = b.LawyerSurename }
                          where (b.CourtCaseNo == courtcaseid) || (a.ID.ToString() == courtcaseid)
                          select a ).Distinct();

            if (lawyers == null)
            {
                return NotFound();
            }
            return await lawyers.ToListAsync();
        }



    }
}
