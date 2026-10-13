using TP2.Models;

namespace TP2.Models.Help
{
    public class ListeCart
    {
        public List<Item> Items { get; private set; }
        public static readonly ListeCart Instance;

        static ListeCart()
        {
            Instance = new ListeCart();
            Instance.Items = new List<Item>();
        }

        protected ListeCart() { }

        public void AddItem(Product prod)
        {
            bool iswhat = false;
            foreach (Item a in Items)
            {
                if (a.Prod.ProductId == prod.ProductId)
                {
                    a.quantite++;
                    iswhat = true;
                    return;
                }
            }

            if (!iswhat)
            {
                Item newItem = new Item(prod);
                newItem.quantite = 1;
                Items.Add(newItem);
            }
        }

        public void SetLessOneItem(Product prod)
        {
            foreach (Item a in Items)
            {
                if (a.Prod.ProductId == prod.ProductId)
                {
                    if (a.quantite <= 1)
                    {
                        RemoveItem(a.Prod);
                        return;
                    }
                    else
                    {
                        a.quantite--;
                        return;
                    }
                }
            }
        }

        public void SetItemQuantity(Product prod, int quantity)
        {
            if (quantity == 0)
            {
                RemoveItem(prod);
                return;
            }

            foreach (Item a in Items)
            {
                if (a.Prod.ProductId == prod.ProductId)
                {
                    a.quantite = quantity;
                    return;
                }
            }
        }

        public void RemoveItem(Product prod)
        {
            Item t = null;
            foreach (Item a in Items)
            {
                if (a.Prod.ProductId == prod.ProductId)
                {
                    t = a;
                }
            }
            if (t != null)
            {
                Items.Remove(t);
            }
        }

        public float GetSubTotal()
        {
            float subTotal = 0;
            foreach (Item i in Items)
            {
                subTotal += i.TotalPrice;
            }
            return subTotal;
        }
    }
}