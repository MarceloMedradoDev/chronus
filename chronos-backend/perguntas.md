Qual é o efeito prático de habilitar Nullable Reference Types (<Nullable>enable</Nullable>) no projeto?

Resposta obrigatória
O compilador passa a emitir avisos sobre possíveis null em referências e usos inconsistentes.
Converte todas as referências em readonly por padrão.
Lança exceção automaticamente ao acessar referência possivelmente nula.
Desabilita atributos [NotNull] e [MaybeNull].

Dado using var s = new MemoryStream(); fora de um bloco using, o que acontece?

Resposta obrigatória
Nada acontece; using var não chama Dispose().
O objeto é descartado imediatamente após a linha.
O objeto é descartado apenas ao final do assembly.
O objeto é descartado no final do scope atual, como se fosse using(...) {}.

Qual afirmativa sobre record é mais correta por padrão?

Resposta obrigatória
record usa igualdade por referência, igual a class.
record gera igualdade estrutural (por valor) e ToString() útil.
record só pode ser immutable e exige init em todas as props.
record só existe para pattern matching; não tem construtor.

Sobre async void, qual uso é aceitável?

Resposta obrigatória
Serviços de domínio para “fire-and-forget”.
Handlers de eventos onde não há chamada aguardando retorno.
Métodos de biblioteca que retornam resultado assíncrono.
Controladores de API para evitar deadlocks.

Qual é a diferença correta entre Task e ValueTask?

Resposta obrigatória
ValueTask permite múltiplos await de forma segura.
Task não pode ser aguardado com await.
ValueTask sempre é mais rápido e deve substituir Task.
ValueTask evita alocações quando muitos resultados já estão disponíveis de forma síncrona.

Dado Span<T>, qual limitação é verdadeira?

Resposta obrigatória
Pode ser boxed para object.
Pode atravessar async/await livremente.
Pode ser armazenado em campos de classe sem restrições.
É um tipo ref struct e não pode ser capturado em closures que escapem da stack.

Qual declaração é mais correta sobre lock em código assíncrono?

Resposta obrigatória
lock é o mesmo que Monitor.TryEnter com await.
lock funciona perfeitamente através de await.
lock não atravessa await; prefira async coordination (p.ex. SemaphoreSlim).
Use lock com async e await para simplificar.

Com IAsyncEnumerable<T>, qual forma de consumir é idiomática?

Resposta obrigatória
enumerable.ToList()
Task.WhenAll(enumerable) diretamente
foreach (var x in enumerable)
await foreach (var x in enumerable)
Dado o código abaixo, qual resultado é correto?

var actions = new List<Action>();
for (int i = 0; i < 3; i++)
actions.Add(() => Console.Write(i));
actions.ForEach(a => a());

Resposta obrigatória
Compila, mas lança exceção.
Imprime 012.
Imprime 222.
Com C# atual, o foreach corrige automaticamente para 012.

Sobre Minimal APIs no ASP.NET Core moderno, qual é a mais correta?

Resposta obrigatória
São incompatíveis com Swagger.
Não suportam filters nem conventions.
São exclusivas para self-hosted e não usam Kestrel.
Permitem injeção por parâmetros de handler (p.ex. HttpContext, serviços) sem Controller.

required em propriedades (C# 11) implica que:

Resposta obrigatória
Atributos de validação não são mais necessários.
A propriedade torna-se readonly.
O runtime lança MissingMemberException se não setar.
O compilador exige inicialização em inicializador/with/construtor, gerando warnings se faltar.


Qual é o efeito de in em parâmetros de métodos?

Resposta obrigatória
Passa por referência somente-leitura, útil para evitar cópias de structs grandes.
Igual ao ref tradicional.
Passa por referência permitindo escrita.
Passa por cópia garantindo imutabilidade.

Qual opção descreve corretamente readonly struct?

Resposta obrigatória
Garante semântica imutável e pode reduzir cópias defensivas, melhorando desempenho.
Impede qualquer campo mutável, inclusive ref.
Não pode ter métodos.
É idêntico a record struct.

Em logging estruturado com ILogger, qual prática reduz alocações?

Resposta obrigatória
Usar message templates com placeholders (LogInformation("User {Id}", id)).
Chamar ToString() manualmente nos objetos.
Concatenar strings com +.
Usar string.Format sempre.

Qual é o problema clássico de criar HttpClient por requisição em ASP.NET Core?

Resposta obrigatória
Não funciona com async.
Não suporta HTTP/2.
Pode esgotar sockets por falta de reuso de HttpMessageHandler.
Cria deadlocks de UI.

Sobre LINQ, qual afirmação é correta?

Resposta obrigatória
OrderBy nunca materializa.
Todas as consultas são executadas imediatamente.
Select e Where usam execução adiada; operações terminais como ToList() materializam.
AsEnumerable() sempre avalia a consulta.

Qual é o risco de ConfigureAwait(false) em bibliotecas?

Resposta obrigatória
Faz await se comportar como Task.Result.
Desabilita exceções de AggregateException.
Sempre quebra HttpContext.
Em bibliotecas, costuma ser recomendado para evitar captura de contexto, mas deve-se conhecer APIs que exigem contexto (UI/ASP.NET).

Qual é a forma correta de implementar Equality eficiente em um record struct?

Resposta obrigatória
record struct já tem igualdade por valor e GetHashCode() gerados, ajustáveis com membros.
Depender de referência para comparar.
Confiar em object.Equals sem sobrescrever.
Apenas com IEquatable<T> manual.

Sobre pattern matching, qual é verdadeiro?

Resposta obrigatória
Relational patterns (>, <) podem ser combinados com logical patterns (and, or) em C# moderno.
Switch expressions não suportam guards (when).
Descartes _ não podem ser usados em switch.
is not é inválido com tipos nulos.

Qual é a consequência de marcar um tipo como ref struct (ex.: Span<T>, Utf8JsonReader)?

Resposta obrigatória
Pode ser campo de classe sem restrição.
Pode ser armazenado no heap via boxing.
Tem restrições de lifetime: não pode escapar da stack, não pode ser boxed, não pode atravessar await/iterators.
Não pode implementar interfaces.

