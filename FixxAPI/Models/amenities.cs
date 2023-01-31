using System.ComponentModel.DataAnnotations;

namespace FixxAPI.Models
{
    public class amenities
    {
        [Key]
        public int id { get; set; }
        public Guid user_id { get; set; }
        public bool cleaning_products { get; set; }
        public bool shampoo { get; set; }
        public bool hot_water { get; set; }
        public bool shower_gel { get; set; }
        public bool essentials { get; set; }
        public bool hangers { get; set; }
        public bool bed_linen { get; set; }
        public bool extra_pillows_blankets { get; set; }
        public bool room_darkening_shades { get; set; }
        public bool iron { get; set; }
        public bool drying_rack { get; set; }
        public bool mosquito_net { get; set; }
        public bool clothing_storage { get; set; }
        public bool ethernet_connection { get; set; }
        public bool tV_with_cable { get; set; }
        public bool security_cameras { get; set; }
        public bool smoke_alarm { get; set; }
        public bool carbon_monoxide_alarm { get; set; }
        public bool fire_extinguisher { get; set; }
        public bool first_aid_kit { get; set; }
        public bool wifi { get; set; }
        public bool dedicated_workspace { get; set; }
        public bool kitchen { get; set; }
        public bool refrigirator { get; set; }
        public bool microwave { get; set; }
        public bool cooking_basics { get; set; }
        public bool freezer { get; set; }
        public bool electric_stove { get; set; }
        public bool oven { get; set; }
        public bool hot_water_Kettle { get; set; }
        public bool coffee_maker { get; set; }
        public bool wine_glasses { get; set; }
        public bool toaster { get; set; }
        public bool coffee { get; set; }
        public bool private_entrance { get; set; }
        public bool laundromat_nearby { get; set; }
        public bool private_balcony { get; set; }
        public bool outdoor_furniture { get; set; }
        public bool free_parking { get; set; }
        public bool building_staff { get; set; }
        public bool self_check_in { get; set; }
        public bool washer { get; set; }
        public bool air_conditioning { get; set; }
        public bool hair_dryer { get; set; }
        public bool heating { get; set; }
    }
}
