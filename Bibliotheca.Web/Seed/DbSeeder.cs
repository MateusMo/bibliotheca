using Bibliotheca.Application.InfraServices;
using Bibliotheca.Data.Context;
using Bibliotheca.Domain.Domains;
using Bibliotheca.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Bibliotheca.Web.Seed;

/// <summary>
/// Popula o banco com dados fake para desenvolvimento.
/// - Usuário padrão: sempre garantido (checado por e-mail), independente do resto já ter sido populado.
/// - Massa de dados fake (usuários aleatórios, livros, comentários): só roda se ainda não existir nenhum livro.
/// Chamar apenas em ambiente Development (ver Program.cs).
/// </summary>
public static class DbSeeder
{
    private static readonly Random Rnd = new(20260701);

    // ---------------------------------------------------------------
    // USUÁRIO PADRÃO
    // Credenciais fixas e fictícias, só para sempre ter um login pronto em dev.
    // NUNCA usar esse padrão de senha/e-mail em produção.
    // ---------------------------------------------------------------
    private const string DefaultUserEmail = "admin@bibliotheca.dev";
    private const string DefaultUserPassword = "Admin123!";
    private const string DefaultUserName = "Administrador Bibliotheca";

    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BibliothecaContext>();
        var crypto = scope.ServiceProvider.GetRequiredService<ICrypto>();

        // Garante o usuário padrão independentemente do resto do seed já ter rodado.
        await EnsureDefaultUserAsync(context, crypto);

        // Massa de dados fake: só gera uma vez (verifica pela existência de livros,
        // não de usuários, já que o usuário padrão sozinho não deve contar como "já seedado").
        if (await context.Books.AnyAsync())
            return;

        var users = BuildUsers(crypto, 25);
        await context.Users.AddRangeAsync(users);

        var profiles = BuildProfiles(users);
        await context.Profiles.AddRangeAsync(profiles);

        var books = BuildBooks(users, 150);
        await context.Books.AddRangeAsync(books);

        var comments = BuildComments(users, books, 350);
        await context.Comments.AddRangeAsync(comments);

        await context.SaveChangesAsync();
    }

    private static async Task EnsureDefaultUserAsync(BibliothecaContext context, ICrypto crypto)
    {
        var exists = await context.Users.AnyAsync(u => u.Email == DefaultUserEmail);
        if (exists)
            return;

        var defaultUser = new User
        {
            Id = Guid.NewGuid(),
            Name = DefaultUserName,
            Email = DefaultUserEmail,
            Password = crypto.HashPassword(DefaultUserPassword),
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        var defaultProfile = new Profile
        {
            Id = Guid.NewGuid(),
            UserId = defaultUser.Id,
            Description = "Conta padrão de desenvolvimento, criada automaticamente pelo seed.",
            Contact = "contato+admin@bibliotheca.dev",
            IsActive = true,
            CreatedAt = defaultUser.CreatedAt,
            UpdatedAt = defaultUser.UpdatedAt
        };

        await context.Users.AddAsync(defaultUser);
        await context.Profiles.AddAsync(defaultProfile);
        await context.SaveChangesAsync();
    }

    // ---------------------------------------------------------------
    // AUTHOR NAMES (agora é só texto pra popular o campo Book.Author)
    // ---------------------------------------------------------------
    private static readonly string[] AuthorNames =
    {
        "Machado de Assis", "Clarice Lispector", "Jorge Amado", "João Guimarães Rosa",
        "Carlos Drummond de Andrade", "Cecília Meireles", "Graciliano Ramos", "José de Alencar",
        "Lima Barreto", "Euclides da Cunha", "Fernando Pessoa", "José Saramago",
        "Eça de Queirós", "Luís de Camões", "Miguel de Cervantes", "Gabriel García Márquez",
        "Jorge Luis Borges", "Pablo Neruda", "Julio Cortázar", "Victor Hugo",
        "Honoré de Balzac", "Gustave Flaubert", "Marcel Proust", "Charles Dickens",
        "Jane Austen", "Virginia Woolf", "Franz Kafka", "Fiódor Dostoiévski",
        "Liev Tolstói", "Anton Tchekhov", "Oscar Wilde", "Edgar Allan Poe",
        "Herman Melville", "Mark Twain", "Emily Dickinson", "Walt Whitman",
        "Dante Alighieri", "Johann Wolfgang von Goethe", "Friedrich Nietzsche", "Miguel Torga"
    };

    // ---------------------------------------------------------------
    // USERS
    // ---------------------------------------------------------------
    private static readonly string[] FirstNames =
    {
        "Mateus", "Ana", "Pedro", "Beatriz", "Lucas", "Mariana", "Rafael", "Camila",
        "Gabriel", "Larissa", "Thiago", "Fernanda", "Bruno", "Juliana", "Felipe",
        "Patrícia", "André", "Carolina", "Diego", "Renata", "Vinícius", "Aline",
        "Rodrigo", "Isabela", "Gustavo"
    };

    private static readonly string[] LastNames =
    {
        "Silva", "Souza", "Oliveira", "Santos", "Pereira", "Costa", "Rodrigues",
        "Almeida", "Nascimento", "Carvalho", "Gomes", "Martins", "Araújo", "Melo",
        "Barbosa", "Ribeiro", "Cardoso", "Teixeira", "Moreira", "Correia"
    };

    private static List<User> BuildUsers(ICrypto crypto, int count)
    {
        var passwordHash = crypto.HashPassword("Teste123!");
        var users = new List<User>();

        for (var i = 0; i < count; i++)
        {
            var first = FirstNames[i % FirstNames.Length];
            var last = LastNames[Rnd.Next(LastNames.Length)];
            var name = $"{first} {last}";

            users.Add(new User
            {
                Id = Guid.NewGuid(),
                Name = name,
                Email = $"{first.ToLowerInvariant()}.{last.ToLowerInvariant()}{i}@bibliotheca.dev",
                Password = passwordHash,
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-Rnd.Next(1, 400)),
                UpdatedAt = DateTimeOffset.UtcNow
            });
        }

        return users;
    }

    // ---------------------------------------------------------------
    // PROFILES
    // ---------------------------------------------------------------
    private static readonly string[] Bios =
    {
        "Colecionador apaixonado por primeiras edições.",
        "Bibliófilo há mais de 20 anos, focado em literatura brasileira.",
        "Pesquisador universitário interessado em edições raras.",
        "Sebista aposentado, ainda garimpando exemplares antigos.",
        "Leitor voraz e colecionador de clássicos da literatura mundial.",
        "Guarda a biblioteca da família há três gerações."
    };

    private static List<Profile> BuildProfiles(List<User> users)
    {
        return users.Select(u => new Profile
        {
            Id = Guid.NewGuid(),
            UserId = u.Id,
            Description = Bios[Rnd.Next(Bios.Length)],
            Contact = $"contato+{u.Name.Split(' ')[0].ToLowerInvariant()}@bibliotheca.dev",
            IsActive = true,
            CreatedAt = u.CreatedAt,
            UpdatedAt = DateTimeOffset.UtcNow
        }).ToList();
    }

    // ---------------------------------------------------------------
    // BOOKS
    // ---------------------------------------------------------------
    private static readonly string[] TitlePrefix =
    {
        "A Sombra", "O Silêncio", "Cartas", "Memórias", "O Último Capítulo",
        "A Casa", "O Jardim", "Confissões", "A Travessia", "O Enigma",
        "Diário", "A Rosa", "O Espelho", "Sonhos", "A Viagem", "O Tempo",
        "Fragmentos", "A Cidade", "O Labirinto", "Ecos"
    };

    private static readonly string[] TitleSuffix =
    {
        "do Vento", "da Noite", "de um Sonhador", "Perdidas", "do Mar",
        "Esquecida", "Secreto", "de um Louco", "Interrompida", "Eterno",
        "Íntimo", "Vermelha", "Quebrado", "Adiados", "ao Fim do Mundo",
        "Suspenso", "do Passado", "Invisível", "sem Saída", "Distantes"
    };

    private static readonly string[] Publishers =
    {
        "Companhia das Letras", "Editora Globo", "Record", "Rocco",
        "Nova Fronteira", "Martins Fontes", "Editora 34", "Cosac Naify",
        "Penguin Companhia", "Todavia", "Autêntica", "Editora Zahar",
        "Editora Ática", "Objetiva", "Alfaguara", "Bertrand Brasil"
    };

    private static readonly string[] Descriptions =
    {
        "Um exemplar com marcas do tempo, mas em ótimo estado de leitura.",
        "Edição que circulou pouco no mercado, hoje difícil de encontrar.",
        "Faz parte de uma coleção maior sobre literatura do período.",
        "Adquirido em sebo, com dedicatória do antigo proprietário.",
        "Capa dura, ilustrações internas e páginas bem conservadas."
    };

    private static List<Book> BuildBooks(List<User> users, int count)
    {
        var books = new List<Book>();
        var languages = Enum.GetValues<LanguageEnum>();

        for (var i = 0; i < count; i++)
        {
            var name = $"{TitlePrefix[Rnd.Next(TitlePrefix.Length)]} {TitleSuffix[Rnd.Next(TitleSuffix.Length)]}";
            var user = users[Rnd.Next(users.Count)];

            var book = new Book
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                User = user,
                IsOwner = Rnd.Next(0, 10) > 1, // ~80% possui de fato
                Name = name,
                Author = AuthorNames[Rnd.Next(AuthorNames.Length)],
                Description = Descriptions[Rnd.Next(Descriptions.Length)],
                PublicationYear = Rnd.Next(1850, 2026),
                Photos = Array.Empty<string>(),
                LanguageEnum = languages[Rnd.Next(languages.Length)],
                Publisher = Publishers[Rnd.Next(Publishers.Length)],
                ISBN = $"978-65-{i:D4}-{Rnd.Next(100, 999)}-{Rnd.Next(0, 9)}",
                Pages = Rnd.Next(80, 900),
                EstimatedValue = Math.Round((decimal)(Rnd.NextDouble() * 980 + 20), 2),
                ConditionEnum = (BookConditionEnum)Rnd.Next(1, 7),
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-Rnd.Next(1, 400)),
                UpdatedAt = DateTimeOffset.UtcNow
            };

            books.Add(book);
        }

        return books;
    }

    // ---------------------------------------------------------------
    // COMMENTS
    // ---------------------------------------------------------------
    private static readonly string[] CommentTemplates =
    {
        "Exemplar impecável, adorei os detalhes da encadernação.",
        "Tenho uma edição parecida, mas com a capa diferente. Muito bonito!",
        "Já procuro essa edição há anos, que inveja boa!",
        "A conservação está excelente para a idade do livro.",
        "Você sabe se essa é realmente a primeira tiragem?",
        "Parabéns pelo cuidado com o acervo!",
        "Esse exemplar tem alguma dedicatória?",
        "Comprei um parecido em um sebo ano passado.",
        "Edição rara, poucos exemplares circulando hoje em dia.",
        "Ótima aquisição, o valor de mercado só tende a subir."
    };

    private static List<Comment> BuildComments(List<User> users, List<Book> books, int count)
    {
        var comments = new List<Comment>();

        for (var i = 0; i < count; i++)
        {
            var book = books[Rnd.Next(books.Count)];
            var user = users[Rnd.Next(users.Count)];

            comments.Add(new Comment
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                User = user,
                BookId = book.Id,
                Book = book,
                Content = CommentTemplates[Rnd.Next(CommentTemplates.Length)],
                Link = Rnd.Next(0, 5) == 0 ? "https://exemplo.com/referencia" : string.Empty,
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-Rnd.Next(1, 300)),
                UpdatedAt = DateTimeOffset.UtcNow
            });
        }

        return comments;
    }
}