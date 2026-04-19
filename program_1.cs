using System;
using System.Threading;

class Table
{
    private SemaphoreSlim[] forks;
    private SemaphoreSlim waiter; 
    public Table(int numberOfForks)
    {
        forks = new SemaphoreSlim[numberOfForks];
        for (int i = 0; i < numberOfForks; i++)
        {
            forks[i] = new SemaphoreSlim(1, 1);
        }
        waiter = new SemaphoreSlim(2, 2);
    }
    public void AskWaiterPermission()
    {
        waiter.Wait();
    }
    public void ReturnWaiterPermission()
    {
        waiter.Release();
    }
    public void TakeFork(int forkIndex)
    {
        forks[forkIndex].Wait();
    }
    public void ReleaseFork(int forkIndex)
    {
        forks[forkIndex].Release();
    }
}

class Philosopher
{
    private const int Total_Meals = 5;
    private readonly int philosopherId;
    private readonly int leftFork;
    private readonly int rightFork;
    private readonly Table table;
    private readonly Thread thread;

    public Philosopher(int philosopherId, Table table)
    {
        this.philosopherId = philosopherId;
        this.table = table;
        this.leftFork = philosopherId;
        this.rightFork = (philosopherId + 1) % 5;
        
        this.thread = new Thread(Run);
    }
    public void Start()
    {
        thread.Start();
    }
    private void Run()
    {
        for (int meal = 1; meal <= Total_Meals; meal++)
        {
            Think(meal);
            table.AskWaiterPermission(); 
            
            PickUpForks();
            Eat(meal);
            ReleaseForks();
            table.ReturnWaiterPermission();
        }
        Console.WriteLine($"--- Філософ {philosopherId} завершив усі {Total_Meals} прийоми їжі ---");
    }
    private void Think(int meal)
    {
        Console.WriteLine($"Філософ {philosopherId} розмірковує      [ітерація {meal}/{Total_Meals}]");
    }
    private void Eat(int meal)
    {
        Console.WriteLine($"Філософ {philosopherId} їсть             [ітерація {meal}/{Total_Meals}]");
    }
    private void PickUpForks()
    {
        table.TakeFork(leftFork);  
        table.TakeFork(rightFork);
    }
    private void ReleaseForks()
    {
        table.ReleaseFork(rightFork);
        table.ReleaseFork(leftFork);
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        int numberOfPhilosophers = 5;
        Table table = new Table(numberOfPhilosophers);
        Philosopher[] philosophers = new Philosopher[numberOfPhilosophers];
        for (int i = 0; i < numberOfPhilosophers; i++)
        {
            philosophers[i] = new Philosopher(i, table);
        }
        for (int i = 0; i < numberOfPhilosophers; i++)
        {
            philosophers[i].Start();
        }        
    }
}
