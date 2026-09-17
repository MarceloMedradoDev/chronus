export function calcularDiasUteis(mes: number, ano: number): { uteis: number; naoUteis: number, arrayDiasUteis: number[] } {
    const feriados: Date[] = [
        new Date(ano, 0, 1),   // Confraternização Universal
        new Date(ano, 3, 18),  // Sexta feira santa
        new Date(ano, 3, 21),  // Tiradentes
        new Date(ano, 2, 4),  // Carnaval
        new Date(ano, 4, 1),   // Dia do Trabalho
        new Date(ano, 8, 7),   // Independência
        new Date(ano, 9, 12),  // Nossa Senhora Aparecida
        new Date(ano, 10, 2),  // Finados
        new Date(ano, 10, 15), // Proclamação da República
        new Date(ano, 11, 25), // Natal
        new Date(ano, 10, 20), // Consciência Negra
    ];
 
    const diasNoMes = new Date(ano, mes, 0).getDate(); // mês começa em 1, então 0 pega o último dia do mês anterior
    let uteis = 0;
    let naoUteis = 0;
    let arrayDiasUteis: number[] = [];
 
    for (let dia = 1; dia <= diasNoMes; dia++) {
        const data = new Date(ano, mes - 1, dia); // mês em JS começa do 0
        const diaSemana = data.getDay(); // 0 = domingo, 6 = sábado
 
        const fimDeSemana = diaSemana === 0 || diaSemana === 6;
        const feriado = feriados.some(f => f.getTime() === data.getTime());
 
        if (!fimDeSemana && !feriado) {
            arrayDiasUteis.push(dia);
            uteis++;
        } else {
            naoUteis++;
        }
    }
 
    return { uteis, naoUteis, arrayDiasUteis };
}