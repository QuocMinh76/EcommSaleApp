using EcommSale.Models;

namespace EcommSale.Models
{
	// Cái này chỉ dùng cho cart trong mỗi session, không lưu xuống database
	public class CartItemVm
	{
		public Product Product { get; set; }
		public int Quantity { get; set; }
	}
}