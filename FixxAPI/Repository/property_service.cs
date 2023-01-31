using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using FixxAPI.DTOs;
using FixxAPI.Helper;
using FixxAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace FixxAPI.Repository
{
    public class property_service : Iproperty_service
    {
        private readonly Data_Context _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public property_service(Data_Context context, IConfiguration configuration, IHttpContextAccessor http)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = http;
        }

        public string get_logged_in_role()
        {
            return _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Role);            
        }

        public async Task<properties> get_property_by_user()
        {            
            string email = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.users.FirstOrDefaultAsync(x => x.email == email);
            Guid user_id = user!.uuid;
            var result = await _context.properties.FirstOrDefaultAsync(x => x.user_id == user_id);
            return result!;
        }

        public async Task<properties> add_property_initial(property_create_dto dto)
        {            
            string email = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.users.FirstOrDefaultAsync(x => x.email == email);
            Guid user_id = user!.uuid;

            var property = new properties
            {
                user_id = user_id,
                name = dto.name,
                description = dto.description,
                pay_per_night = dto.pay_per_night,
                adult_count = dto.adult_count,
                children_count = dto.children_count,
                bathroom_count = dto.bathroom_count,
                bedroom_count = dto.bedroom_count,
                property_type_id = dto.property_type_id,
                property_space = dto.property_space,
                property_category_id = dto.property_category_id,
                status = dto.status,
                created_on = DateTime.Now
            };

            var prop_ = _context.properties.Add(property);
            await _context.SaveChangesAsync();
            return prop_.Entity;
        }

        public async Task<properties> update_properties(property_update_dto dto)
        {            
            string email = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.users.FirstOrDefaultAsync(x => x.email == email);
            Guid user_id = user!.uuid;

            var property = await _context.properties.FirstOrDefaultAsync(x => x.user_id == user_id);

            if(property != null)
            {
                property.name = dto.name;
                property.description = dto.description;
                property.pay_per_night = dto.pay_per_night;
                property.adult_count = dto.adult_count;
                property.children_count = dto.children_count;
                property.bathroom_count = dto.bathroom_count;
                property.bedroom_count = dto.bedroom_count;
                property.property_type_id = dto.property_type_id;
                property.property_space = dto.property_space;
                property.property_category_id = dto.property_category_id;
                property.status = dto.status;

                await _context.SaveChangesAsync();
                return property;
            }
            return null!;
        }

        public async Task<bool> delete_properties()
        {
            string email = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.users.FirstOrDefaultAsync(x => x.email == email);
            Guid user_id = user!.uuid;

            var property = await _context.properties.FirstOrDefaultAsync(x => x.user_id == user_id);
            if (property != null)
            {
                _context.properties.Remove(property);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task UploadImagesToS3Bucket(List<MemoryStream> imageStreams)
        {
            string email = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.users.FirstOrDefaultAsync(x => x.email == email);
            Guid user_id = user!.uuid;

            string access_key = _configuration.GetSection("AWS_KEYS:ACCESS_KEY_ID").Value!;
            string secret_key = _configuration.GetSection("AWS_KEYS:SECRET_ACCESS_KEY").Value!;
            var credentials = new BasicAWSCredentials(access_key, secret_key);

            var config = new AmazonS3Config()
            {
                RegionEndpoint = Amazon.RegionEndpoint.USEast2
            };

            var s3Client = new AmazonS3Client(credentials, config);

            string bucketName = "fixxbucket";
            var folderName = user_id.ToString() + "/";

            // Check if folder already exists
            var listRequest = new ListObjectsRequest
            {
                BucketName = bucketName,
                Prefix = folderName
            };
            var listResponse = await s3Client.ListObjectsAsync(listRequest);
            if (listResponse.S3Objects.Count == 0)
            {
                // Create a "folder" by creating a "folder" object with a key name that ends with a "/"
                var folderKey = folderName;
                if (!folderKey.EndsWith("/"))
                    folderKey += "/";
                await s3Client.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = folderKey,
                    ContentBody = ""
                });
            }

            // Upload images to the "folder"            
            foreach (var imageStream in imageStreams)
            {
                var imagePath = Guid.NewGuid().ToString();
                using (imageStream)
                {
                    var fileName = Path.GetFileName(imagePath);
                    var key = folderName + fileName;
                    await s3Client.PutObjectAsync(new PutObjectRequest
                    {
                        BucketName = bucketName,
                        Key = key,
                        InputStream = imageStream
                    });
                }
            }
        }
        public async Task<List<S3Object>> GetImagesFromS3Bucket()
        {
            string email = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.users.FirstOrDefaultAsync(x => x.email == email);
            Guid user_id = user!.uuid;

            string access_key = _configuration.GetSection("AWS_KEYS:ACCESS_KEY_ID").Value!;
            string secret_key = _configuration.GetSection("AWS_KEYS:SECRET_ACCESS_KEY").Value!;
            var credentials = new BasicAWSCredentials(access_key, secret_key);

            var config = new AmazonS3Config()
            {
                RegionEndpoint = Amazon.RegionEndpoint.USEast2
            };

            var s3Client = new AmazonS3Client(credentials, config);
            string bucketName = "fixxbucket";
            string folderName = user_id.ToString() + "/";

            var files = new List<S3Object>();

            ListObjectsRequest listObjectsRequest = new ListObjectsRequest
            {
                BucketName = bucketName,
                Prefix = folderName
            };

            ListObjectsResponse listObjectsResponse;

            do
            {
                listObjectsResponse = await s3Client.ListObjectsAsync(listObjectsRequest);
                files.AddRange(listObjectsResponse.S3Objects);

            } while (listObjectsResponse.IsTruncated);

            return files;
        }

        public async Task<amenities> get_amenities()
        {
            string email = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.users.FirstOrDefaultAsync(x => x.email == email);
            Guid user_id = user!.uuid;

            var amenity = await _context.amenities.FirstOrDefaultAsync(x => x.user_id == user_id);
            return amenity!;
        }

        public async Task<amenities> add_ammenity(amenity_add_dto dto)
        {
            string email = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.users.FirstOrDefaultAsync(x => x.email == email);
            Guid user_id = user!.uuid;

            var amenity = new amenities
            {
                user_id = user_id,
                cleaning_products = dto.cleaning_products,
                shampoo = dto.shampoo,
                hot_water = dto.hot_water,
                shower_gel = dto.shower_gel,
                essentials = dto.essentials,
                hangers = dto.hangers,
                bed_linen = dto.bed_linen,
                extra_pillows_blankets = dto.extra_pillows_blankets,
                room_darkening_shades = dto.room_darkening_shades,
                iron = dto.iron,
                drying_rack = dto.drying_rack,
                mosquito_net = dto.mosquito_net,
                clothing_storage = dto.clothing_storage,
                ethernet_connection = dto.ethernet_connection,
                tV_with_cable = dto.tV_with_cable,
                security_cameras = dto.security_cameras,
                smoke_alarm = dto.smoke_alarm,
                carbon_monoxide_alarm = dto.carbon_monoxide_alarm,
                fire_extinguisher = dto.fire_extinguisher,
                first_aid_kit = dto.first_aid_kit,
                wifi = dto.wifi,
                dedicated_workspace = dto.dedicated_workspace,
                kitchen = dto.kitchen,
                refrigirator = dto.refrigirator,
                microwave = dto.microwave,
                cooking_basics = dto.cooking_basics,
                freezer = dto.freezer,
                electric_stove = dto.electric_stove,
                oven = dto.oven,
                hot_water_Kettle = dto.hot_water_Kettle,
                coffee_maker = dto.coffee_maker,
                wine_glasses = dto.wine_glasses,
                toaster = dto.toaster,
                coffee = dto.coffee,
                private_entrance = dto.private_entrance,
                laundromat_nearby = dto.laundromat_nearby,
                private_balcony = dto.private_balcony,
                outdoor_furniture = dto.outdoor_furniture,
                free_parking = dto.free_parking,
                building_staff = dto.building_staff,
                self_check_in = dto.self_check_in,
                washer = dto.washer,
                air_conditioning = dto.air_conditioning,
                hair_dryer = dto.hair_dryer,
                heating = dto.heating,
            };

            var prop_ = _context.amenities.Add(amenity);
            await _context.SaveChangesAsync();
            return prop_.Entity;
        }

        public async Task<amenities> update_ammenity(amenity_add_dto dto)
        {
            string email = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.users.FirstOrDefaultAsync(x => x.email == email);
            Guid user_id = user!.uuid;

            var amenity = await _context.amenities.FirstOrDefaultAsync(x => x.user_id == user_id);

            if (amenity != null)
            {
                amenity.cleaning_products = dto.cleaning_products;
                amenity.shampoo = dto.shampoo;
                amenity.hot_water = dto.hot_water;
                amenity.shower_gel = dto.shower_gel;
                amenity.essentials = dto.essentials;
                amenity.hangers = dto.hangers;
                amenity.bed_linen = dto.bed_linen;
                amenity.extra_pillows_blankets = dto.extra_pillows_blankets;
                amenity.room_darkening_shades = dto.room_darkening_shades;
                amenity.iron = dto.iron;
                amenity.drying_rack = dto.drying_rack;
                amenity.mosquito_net = dto.mosquito_net;
                amenity.clothing_storage = dto.clothing_storage;
                amenity.ethernet_connection = dto.ethernet_connection;
                amenity.tV_with_cable = dto.tV_with_cable;
                amenity.security_cameras = dto.security_cameras;
                amenity.smoke_alarm = dto.smoke_alarm;
                amenity.carbon_monoxide_alarm = dto.carbon_monoxide_alarm;
                amenity.fire_extinguisher = dto.fire_extinguisher;
                amenity.first_aid_kit = dto.first_aid_kit;
                amenity.wifi = dto.wifi;
                amenity.dedicated_workspace = dto.dedicated_workspace;
                amenity.kitchen = dto.kitchen;
                amenity.refrigirator = dto.refrigirator;
                amenity.microwave = dto.microwave;
                amenity.cooking_basics = dto.cooking_basics;
                amenity.freezer = dto.freezer;
                amenity.electric_stove = dto.electric_stove;
                amenity.oven = dto.oven;
                amenity.hot_water_Kettle = dto.hot_water_Kettle;
                amenity.coffee_maker = dto.coffee_maker;
                amenity.wine_glasses = dto.wine_glasses;
                amenity.toaster = dto.toaster;
                amenity.coffee = dto.coffee;
                amenity.private_entrance = dto.private_entrance;
                amenity.laundromat_nearby = dto.laundromat_nearby;
                amenity.private_balcony = dto.private_balcony;
                amenity.outdoor_furniture = dto.outdoor_furniture;
                amenity.free_parking = dto.free_parking;
                amenity.building_staff = dto.building_staff;
                amenity.self_check_in = dto.self_check_in;
                amenity.washer = dto.washer;
                amenity.air_conditioning = dto.air_conditioning;
                amenity.hair_dryer = dto.hair_dryer;
                amenity.heating = dto.heating;

                await _context.SaveChangesAsync();
                return amenity;
            }
            return null!;
        }

        public async Task<bool> delete_amenity()
        {
            string email = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.users.FirstOrDefaultAsync(x => x.email == email);
            Guid user_id = user!.uuid;

            var amenity = await _context.amenities.FirstOrDefaultAsync(x => x.user_id == user_id);
            if (amenity != null)
            {
                _context.amenities.Remove(amenity);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
