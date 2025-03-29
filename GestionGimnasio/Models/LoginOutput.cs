namespace GestionGimnasio.Models
{
    public class LoginOutput
    {

        public int Id { get; set; }
        
        public string Usuario { get; set; }
        public string Nombre_Empleado { get; set; }
        public int Rol { get; set; }
        public int Empleado_Id { get; set; }
        public bool Admin { get; set; }

    }
}
