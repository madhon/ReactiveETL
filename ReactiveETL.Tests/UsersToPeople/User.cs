namespace ReactiveETL.Tests;

public class User
{
    public User()
    {

    }

    public User(int id, string name, string email)
    {
        this.Id = id;
        this.Name = name;
        this.Email = email;
    }

    public string Email { get; set; }

    public int Id { get; set; }

    public string Name { get; set; }
}