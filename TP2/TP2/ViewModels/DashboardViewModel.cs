namespace TP2.ViewModels
{
    public class DashboardViewModel
    {
        // KPIs principaux
        public float TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }

        // Données pour le graphique Ventes par Catégorie (Pie / Doughnut Chart)
        public List<string> CategoryNames { get; set; } = new List<string>();
        public List<float> CategorySales { get; set; } = new List<float>();

        // Données pour le graphique Ventes du Dernier Mois (Bar Chart)
        public List<string> DateLabels { get; set; } = new List<string>();
        public List<float> DailyRevenue { get; set; } = new List<float>();
    }
}