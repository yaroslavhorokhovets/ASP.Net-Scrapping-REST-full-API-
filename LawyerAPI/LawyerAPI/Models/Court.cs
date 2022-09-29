namespace LawyerAPI.Models
{
    public class Court
    {
        public int ID { get; set; }
        public string? Canton { get; set; }
        public string? Codification { get; set; }
        public string? Division { get; set; }

        public string? DivisionId { get; set; }
        public string? CourtId { get; set; }
        public string? JurAnnexe { get; set; }
        public string? JurNum { get; set; }
        public string? TypeJuridiction { get; set; }
        public int TypeJuridictionId { get; set; }
        public string? Country { get; set; }
    }
}
