using MyRestaurant.Models;
using System;

namespace MyRestaurant.Utility
{
    public static class SD
    {
        public const string ManagerUser = "Manager";
        public const string KitchenUser = "Kitchen";
        public const string FrontDeskUser = "Front Desk";
        public const string CustomerUser = "Customer";


        public const string ShoppingCartCount = "ShoppingCartCount";
        public const string ssCopounCode = "CopounCode";


        public static double DiscountPrice(Copoun copoun,double OrderTotalOriginal)
        {
            if(copoun== null)
            {
                return Math.Round(OrderTotalOriginal,2);
            }
            else
            {
                if (copoun.MinimumAmount > OrderTotalOriginal)
                {
                    return Math.Round(OrderTotalOriginal, 2);

                }
                else
                {
                    if (int.Parse(copoun.CopounType) == (int)Copoun.ECopounType.Dollar)
                    {
                        return Math.Round(OrderTotalOriginal -copoun.Discount, 2);

                    }
                    else
                    {
                        return Math.Round(OrderTotalOriginal - (OrderTotalOriginal * (copoun.Discount / 100)), 2);
                    }
                }
            }
        }
    }
}
