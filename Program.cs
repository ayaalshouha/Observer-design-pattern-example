using System;
using System.Diagnostics;
using System.Threading;
using System.Xml.Serialization;


//example1
public class TempsEventArgs : EventArgs
{
    public double OldTempreture { get; set; }
    public double NewTempreture { get; set; }
    public double Difference { get; set; }
    public TempsEventArgs(double OldTemp, double NewTemp) 
    {
        this.NewTempreture = NewTemp;
        this.OldTempreture = OldTemp;
        this.Difference = NewTemp - OldTemp;
    }
}

public class Thermostat
{
    public event EventHandler<TempsEventArgs> onTempretureChanged;

    private double OldTempreture; 
    private double CurrentTempreture;   

    public void RaiseOnTempChanged(double OldTemp, double NewTemp)
    {
        RaiseOnTempChanged(new TempsEventArgs(OldTemp,NewTemp));
    }
    protected virtual void RaiseOnTempChanged(TempsEventArgs e)
    {
        onTempretureChanged?.Invoke(this, e);
    }


    public void SetTempreture(double NewTemp)
    {
        if(CurrentTempreture != NewTemp)
        {
            OldTempreture = CurrentTempreture;
            CurrentTempreture = NewTemp; 
            if(onTempretureChanged != null)
            {
                RaiseOnTempChanged(OldTempreture, CurrentTempreture); 
            }
        }
    }

}

public class Display
{

    public Display(Thermostat thermostat) 
    {
       thermostat.onTempretureChanged += HandleTempretureChanged;
    }
   
    private void HandleTempretureChanged(object sender, TempsEventArgs e)
    {
        Console.WriteLine($"Tempreture Changed: ");
        Console.WriteLine($"Old Tempreture = {e.OldTempreture}C");
        Console.WriteLine($"New Tempreture = {e.NewTempreture}C");
        Console.WriteLine($"Difference = {e.Difference}C"); 
    }
}

public class Print
{
    public void Subscribe(Thermostat thermostat)
    {
        thermostat.onTempretureChanged += HandleTempretureChanged; 
    }
    private void HandleTempretureChanged(object sender, TempsEventArgs e)
    {
        if(e.NewTempreture >= 100)
        {
            Console.WriteLine("Tempreture is Above 100C, AC is ON now!"); 
        }
    }
}


//example2
public class Article
{
    public string Title { get; set; }
    public string Content { get; set; }

    public Article(string title, string content)
    {
        Title = title;
        Content = content;
    }
}

public class NewsPublisher
{   
    public event EventHandler<Article> OnPublishing;

    private string title; 
    private string content;

    public void RaiseOnNewsPublished(string title, string content)
    {
        RaiseOnNewsPublished(new Article(title, content));
    }
    protected virtual void RaiseOnNewsPublished(Article article)
    {
        OnPublishing?.Invoke(this, article);
    }

    public void NewsComing(string title, string content)
    {
        this.title = title;
        this.content = content;
        RaiseOnNewsPublished(this.title, this.content); 
    }

}

public class NewSubscriber
{
    public string Name { get; set; } 

    public NewSubscriber(string name)
    {
        Name = name;
    }

    public void Subscibe(NewsPublisher publisher)
    {
        publisher.OnPublishing += HandleNewNews;
    }

    public void UnSubscibe(NewsPublisher publisher)
    {
        publisher.OnPublishing -= HandleNewNews;
    }
    private void HandleNewNews(object sender, Article article)
    {
        Console.WriteLine($"{this.Name} recieved news: ");
        Console.WriteLine($"Title : {article.Title}");
        Console.WriteLine($"Content: {article.Content}");
        Console.WriteLine(); 
    }
}


//example3
public class OrderEventArgs : EventArgs
{
    public string TrackingNumber { get; set; }
    public double TotalPrice { get; set; }
    public string Email { get; set; }
    
    public OrderEventArgs(string trackingnumber, string email, double totalprice) { 
        TrackingNumber = trackingnumber;
        Email = email;
        TotalPrice = totalprice;
    }
}

public class Order
{
    public event EventHandler<OrderEventArgs> OnSubmition; 

    private void RaiseOnOrderSubmition(string TrackingNumber, string Email, double OrderPrice)
    {
        RaiseOnOrderSubmition(new OrderEventArgs(TrackingNumber, Email, OrderPrice));
    }
    protected virtual void RaiseOnOrderSubmition(OrderEventArgs order)
    {
        OnSubmition?.Invoke(this, order);
    }

    public void Submit(string TrackingNumber, string Email, double OrderPrice)
    {
        RaiseOnOrderSubmition(TrackingNumber, Email, OrderPrice);
    }

}

public class EmailService
{
    public void Subscribe(Order order)
    {
        order.OnSubmition += HandleOrderSubmition; 
    }

    private void HandleOrderSubmition(object sender, OrderEventArgs order)
    {
        Console.WriteLine($"--------Email Service--------");
        Console.WriteLine($"Email sent to {order.Email}!");
        Console.WriteLine($"Your Order has been submitted successfully!");
        Console.WriteLine($"Tracking Number = {order.TrackingNumber}");
        Console.WriteLine($"Total Price = {order.TotalPrice}");
        Console.WriteLine();
    }
}

public class SMSService
{
    public void Subscribe(Order order)
    {
        order.OnSubmition += HandleOrderSubmition;
    }

    private void HandleOrderSubmition(object sender, OrderEventArgs order)
    {
        Console.WriteLine($"--------SMS Service--------");
        Console.WriteLine($"Your Order has been submitted successfully!");
        Console.WriteLine($"You recieved an email to the email address  {order.Email}!");
        Console.WriteLine($"Tracking Number = {order.TrackingNumber}");
        Console.WriteLine($"Total Price = {order.TotalPrice}");
        Console.WriteLine();
    }
}

public class ShippingService
{
    public void Subscribe(Order order)
    {
        order.OnSubmition += HandleOrderSubmition;
    }

    private void HandleOrderSubmition(object sender, OrderEventArgs order)
    {
        Console.WriteLine($"--------Shipping Service--------");
        Console.WriteLine($"Client Email = {order.Email}!");
        Console.WriteLine($"Please prepare items in order number {order.TrackingNumber}!");
        Console.WriteLine($"Please check the total price = {order.TotalPrice}");
        Console.WriteLine();
    }
}

public class Program
{
    static void Main()
    {

        Order order1 = new Order();
        SMSService smsService = new SMSService();
        EmailService emailService = new EmailService();
        ShippingService shippingService = new ShippingService();
        //subscribing to the event 
        smsService.Subscribe(order1);
        emailService.Subscribe(order1); 
        shippingService.Subscribe(order1);
        order1.Submit("ADVV66433", "Client1@ex.com", 87.6);




        NewsPublisher publisher = new NewsPublisher();
        NewSubscriber subscriber1 = new NewSubscriber("NYT (NEWYORK TIMES)");
        NewSubscriber subscriber2 = new NewSubscriber("Washington Post");

        subscriber1.Subscibe(publisher);
        publisher.NewsComing("BreakingNews", "Welcome to our sample code repository! Whether you're a seasoned developer or just getting started, our sample code is designed to help you explore and understand various programming concepts. Each code snippet comes with comprehensive comments and documentation to guide you through its functionality. Feel free to experiment, modify, and integrate these samples into your own projects. If you have any questions or need assistance, our community forum is a great place to connect with other developers and seek help. Happy coding!");

        subscriber1.UnSubscibe(publisher);
        subscriber2.Subscibe(publisher);
        publisher.NewsComing("Tech Updates", "New Microsodt Technology is Coming Soon");




        Thermostat thermo1 = new Thermostat();
        Display display1 = new Display(thermo1);
        Print print1 = new Print();

        //display1.Subscribe(thermo1);
        print1.Subscribe(thermo1);

        thermo1.SetTempreture(23);
        thermo1.SetTempreture(23);
        thermo1.SetTempreture(50);
        thermo1.SetTempreture(80);
        thermo1.SetTempreture(120);

        Console.ReadKey();
    }
}


