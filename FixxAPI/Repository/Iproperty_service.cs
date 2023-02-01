using Amazon.S3.Model;
using FixxAPI.DTOs;
using FixxAPI.Models;

namespace FixxAPI.Repository
{
    public interface Iproperty_service
    {
        public string get_logged_in_role();
        public Task<properties> get_property_by_user();
        public Task<properties> add_property_initial(property_create_dto dto);
        public Task<properties> update_properties(property_update_dto dto);
        public Task<bool> delete_properties();
        public Task UploadImagesToS3Bucket(List<MemoryStream> imageStreams);
        public Task<List<S3Object>> GetImagesFromS3Bucket();
        public Task<amenities> get_amenities();
        public Task<amenities> add_ammenity(amenity_add_dto dto);
        public Task<amenities> update_ammenity(amenity_add_dto dto);
        public Task<bool> delete_amenity();
        public Task<prop_categ_type_amen> get_all_info();
    }
}
