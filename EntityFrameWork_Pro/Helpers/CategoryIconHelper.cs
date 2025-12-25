namespace EntityFrameWork_Pro.Helpers
{
    public static class CategoryIconHelper
    {
        public static string GetCategoryIcon(string category)
        {
            return category?.ToLower() switch
            {
                "wallet" => "fas fa-wallet",
                "phone" => "fas fa-mobile-alt",
                "keys" => "fas fa-key",
                "id" => "fas fa-id-card",
                "bag" => "fas fa-briefcase",
                "laptop" => "fas fa-laptop",
                "watch" => "fas fa-clock",
                "jewelry" => "fas fa-gem",
                "clothing" => "fas fa-tshirt",
                "books" => "fas fa-book",
                "other" => "fas fa-question-circle",
                _ => "fas fa-box"
            };
        }

        public static string GetCategoryColor(string category)
        {
            return category?.ToLower() switch
            {
                "wallet" => "#8e44ad",
                "phone" => "#3498db",
                "keys" => "#f39c12",
                "id" => "#e74c3c",
                "bag" => "#2ecc71",
                "laptop" => "#34495e",
                "watch" => "#16a085",
                "jewelry" => "#e67e22",
                "clothing" => "#9b59b6",
                "books" => "#c0392b",
                "other" => "#95a5a6",
                _ => "#7f8c8d"
            };
        }
    }
}
