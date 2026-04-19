import java.util.concurrent.Semaphore;

class Table {
    private Semaphore[] forks;
    private Semaphore waiter; 

    public Table(int numberOfForks) {
        forks = new Semaphore[numberOfForks];
        for (int i = 0; i < numberOfForks; i++) {
            forks[i] = new Semaphore(1);
        }
        waiter = new Semaphore(4);
    }

    public void askWaiterPermission() {
        try {
            waiter.acquire();
        } catch (InterruptedException e) {
            Thread.currentThread().interrupt();
        }
    }

    public void returnWaiterPermission() {
        waiter.release();
    }

    public void takeFork(int forkIndex) {
        try {
            forks[forkIndex].acquire();
        } catch (InterruptedException e) {
            Thread.currentThread().interrupt();
        }
    }

    public void releaseFork(int forkIndex) {
        forks[forkIndex].release();
    }
}

class Philosopher implements Runnable {
    private static final int TOTAL_MEALS = 5;

    private final int philosopherId;
    private final int leftFork;
    private final int rightFork;
    private final Table table;

    public Philosopher(int philosopherId, Table table) {
        this.philosopherId = philosopherId;
        this.table = table;
        this.leftFork = philosopherId;
        this.rightFork = (philosopherId + 1) % 5;
    }

    @Override
    public void run() {
        for (int meal = 1; meal <= TOTAL_MEALS; meal++) {
            think(meal);
            
            table.askWaiterPermission(); 
            
            pickUpForks();
            eat(meal);
            releaseForks();
            
            table.returnWaiterPermission();
        }
        System.out.println("--- Філософ " + philosopherId + " завершив усі " + TOTAL_MEALS + " прийоми їжі ---");
    }

    private void think(int meal) {
        System.out.printf("Філософ %d розмірковує      [ітерація %d/%d]%n", philosopherId, meal, TOTAL_MEALS);
    }

    private void eat(int meal) {
        System.out.printf("Філософ %d їсть             [ітерація %d/%d]%n", philosopherId, meal, TOTAL_MEALS);
    }

    private void pickUpForks() {
        table.takeFork(leftFork);  
        table.takeFork(rightFork);
    }

    private void releaseForks() {
        table.releaseFork(rightFork);
        table.releaseFork(leftFork);
    }
}

public class Main {
    public static void main(String[] args) {
        int numberOfPhilosophers = 5;
        Table table = new Table(numberOfPhilosophers);
        Philosopher[] philosophers = new Philosopher[numberOfPhilosophers];
        for (int i = 0; i < numberOfPhilosophers; i++) {
            philosophers[i] = new Philosopher(i, table);
        }
        for (int i = 0; i < numberOfPhilosophers; i++) {
            new Thread(philosophers[i]).start();
        }        
    }
}
