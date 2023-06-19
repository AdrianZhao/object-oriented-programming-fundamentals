VendingMachine vendingMachine = new VendingMachine("hahaha");
Console.WriteLine(vendingMachine.SerialNumber);
VendingMachine vendingMachine2 = new VendingMachine("xixixi");
Console.WriteLine(vendingMachine2.SerialNumber);
class VendingMachine
{
    private static int _serialNumber = 0;
    public int SerialNumber { get { return _serialNumber; } }
    private string? _barcode;
    public string? Barcode { get { return _barcode; } }

    private Dictionary<int, int> MoneyFloat { get; }
    private Dictionary<Product, int> Inventory { get; }
    public VendingMachine(string barcode)
    {
        try
        {
            if (!string.IsNullOrEmpty(barcode))
            {
                _barcode = barcode;
            }
            else
            {
                throw new ArgumentNullException(nameof(barcode), "Barcode can not be empty.");
            }
        } catch (Exception e)
        {
            throw new Exception(e.Message);
        }
        _serialNumber++;
        MoneyFloat = new Dictionary<int, int>
        {
            { 1, 20 },
            { 2, 10 },
            { 5, 5 },
            { 10, 5 },
            { 20, 2 }
        };
        Inventory = new Dictionary<Product, int>();
    }
    public void StockItem(Product product, int quantity)
    {
        try
        {
            if (quantity >= 0) 
            {
                if (Inventory.ContainsKey(product))
                {
                    Inventory[product] += quantity;
                }
                else
                {
                    Inventory.Add(product, quantity);
                }
            }
            else 
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be equal or greater than 0.");
            }
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
        }
        Console.WriteLine($"Stocked Items: Name: {product.Name}, Code: {product.Code}, Price: ${product.Price}, Quantity: {Inventory[product]}");
    }
    public void StockFloat(int moneyDenomination, int quantity)
    {
        if (MoneyFloat.ContainsKey(moneyDenomination))
        {
            MoneyFloat[moneyDenomination] += quantity;
        }
        else
        {
            MoneyFloat.Add(moneyDenomination, quantity);
        }
        Console.WriteLine($"Stocked Money: Name: ${moneyDenomination}, Quantity: {MoneyFloat[moneyDenomination]}");
    }

    public string VendItem(string code, List<int> money)
    {
        try
        {
            if (string.IsNullOrEmpty(code) && money.Count > 0)
            {
                // set a null temp product to find if the product exist
                Product tempProduct = null;
                foreach (KeyValuePair<Product, int> item in Inventory)
                {
                    if (item.Key.Code == code)
                    {
                        tempProduct = item.Key;
                        break;
                    }
                }
                if (tempProduct == null)
                {
                    return $"Error, no item with code {code}";
                }
                else if (Inventory[tempProduct] == 0)
                {
                    return "Error: Item is out of stock";
                }
                else
                {
                    int total = 0;
                    foreach (int amount in money)
                    {
                        total += amount;
                    }
                    if (total < tempProduct.Price)
                    {
                        return "Error: insufficient money provided";
                    }
                    else if (total == tempProduct.Price)
                    {
                        return $"Please enjoy your '{tempProduct.Name}'. No change required.";
                    }
                    else
                    {
                        int leftAmount = total - tempProduct.Price;
                        Dictionary<int, int> returnMoneyList = new Dictionary<int, int>();
                        List<int> allMoney = new List<int>(MoneyFloat.Keys);
                        // Sort from big to small
                        allMoney.Sort();
                        allMoney.Reverse();
                        // allMoney.Sort((a, b) => b.CompareTo(a));
                        foreach (int moneyList in allMoney)
                        {
                            int count = MoneyFloat[moneyList];
                            if (leftAmount >= moneyList && count > 0)
                            {
                                int dispense = leftAmount / moneyList;
                                if (count < dispense)
                                {
                                    dispense = count;
                                }
                                MoneyFloat[moneyList] -= dispense;
                                leftAmount -= moneyList * dispense;
                                returnMoneyList.Add(moneyList, dispense);
                            }
                        }
                        if (leftAmount > 0)
                        {
                            return "Unable to dispense change.";
                        }
                        Inventory[tempProduct]--;
                        string change = printChange(returnMoneyList);
                        return $"Please enjoy your '{tempProduct.Name}' and take your change of {change}";
                    }
                }
            }
            else
            {
                throw new Exception("Code or Money is invalid.");
            }
        }
        catch (Exception ex) 
        { 
            throw new Exception(ex.Message); 
        }
    }
    private string printChange(Dictionary<int, int> changeReturned)
    {
        string change = "";
        foreach (KeyValuePair<int, int> returnedIteration in changeReturned)
        {
            change += $"${returnedIteration.Key}x{returnedIteration.Value}, ";
        }
        return change;
    }
}
class Product
{
    private string _name;
    public string Name { get { return _name; } }
    private int _price;
    public int Price { get { return _price; } }
    private string _code;
    public string Code { get { return _code; } }
    public Product(string name, int price, string code)
    {
        try
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name), "Name can not be empty.");
            }
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code), "Code can not be empty.");
            }
            if (price < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(price), "Price has to be 0 or greater than 0.");
            }
        }
        catch (Exception e)
        {
            throw new ArgumentException(e.Message);
        }
        _name = name;
        _price = price;
        _code = code;
    }
}