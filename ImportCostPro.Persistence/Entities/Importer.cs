using ImportCostPro.Persistence.Common;

namespace ImportCostPro.Persistence.Entities
{
    public class Importer : ActivatableBaseEntity 
    {
        
        public required string ComercialName { get; set; }
        public required string RNC { get; set; }
        public int? Phone { get; set; }
        public required string Email { get; set;}
        public string? Address { get; set; }

        // Navigation Properties
        
        // El importador pertenece a un Pais
        public int CountryId { get; set; }

        public Country? Country { get; set; }



        


        

         

   
    }
}