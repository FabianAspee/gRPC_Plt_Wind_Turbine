window.setupCalendarChart = (id, config) => {

    const turbineData = [{ info: "info", values: [{ nameTurbine: "V1", values: [{ settimana: 1, value: 0.02 }, { settimana: 2, value: 0.02 }, { settimana: 3, value: 0.02 }, { settimana: 4, value: 0.32 }] }, { nameTurbine: "V2", values: [{ settimana: 1, value: 0.02 }, { settimana: 2, value: 0.02 }, { settimana: 3, value: 0.02 }, { settimana: 4, value: 0.22 }] }, { nameTurbine: "V3", values: [{ settimana: 1, value: 0.02 }, { settimana: 2, value: 0.02 }, { settimana: 3, value: 0.02 }, { settimana: 4, value: 0.02 }] }, { nameTurbine: "V4", values: [{ settimana: 1, value: 0.02 }, { settimana: 2, value: 0.02 }, { settimana: 3, value: 0.02 }, { settimana: 4, value: 0.12 }] }, { nameTurbine: "V5", values: [{ settimana: 1, value: 6.02 }, { settimana: 2, value: 0.02 }, { settimana: 3, value: 0.02 }, { settimana: 4, value: 0.02 }] }, { nameTurbine: "V6", values: [{ settimana: 1, value: 0.02 }, { settimana: 2, value: 0.02 }, { settimana: 3, value: 0.02 }, { settimana: 4, value: 0.42 }] }, { nameTurbine: "V7", values: [{ settimana: 1, value: 0.02 }, { settimana: 2, value: 0.22 }, { settimana: 3, value: 0.52 }, { settimana: 4, value: 1.02 }] }] }]
    const nameTurbine = turbineData.map(x => x.values.map(turbine => turbine.nameTurbine))[0]
    const helpcolor = turbineData.map(val => val.values.map(turbine => turbine.values.map(x => x.value)).flat()).flat()

    var color = () => {
        const max = d3.quantile(helpcolor, 0.9975, d => Math.abs(d));
        return d3.scaleSequential(d3.interpolatePiYG).domain([-max, +max]);
    }
    const cellSize = 17
    const width = 954
    const height = 319
    const countDay = i => (i + 6) % 7
    const svg = d3.select(`#${id}`)
        .append('svg')
        .attr("viewBox", [10, 10, 420, height])
        .attr("font-family", "sans-serif")
        .attr("font-size", 10);

    const year = svg.selectAll("g")
        .data(() => [turbineData[0].info])
        .join("g")
        .attr("transform", (d, i) => `translate(40.5,${height * i + cellSize * 1.5})`);

    year.append("text")
        .attr("x", -5)
        .attr("y", -4)
        .attr("font-weight", "bold")
        .attr("text-anchor", "end")
        .text(key => key);
    const index = d3.range(1, nameTurbine.length + 1)

    year.append("g")
        .attr("text-anchor", "end")
        .selectAll("text")
        .data(index)
        .join("text")
        .attr("x", -5)
        .attr("y", i => (countDay(i) + 0.5) * cellSize)
        .attr("dy", "0.31em")
        .text(x => nameTurbine[x - 1]);

    const week = {}
    for (var turbines in turbineData) {
        for (var indexT in turbineData[turbines].values) {
            turbineData[turbines].values[indexT].values.forEach(x => {
                if (!(x.settimana in week)) {
                    week[x.settimana] = [x]
                }
                else {
                    var value = week[x.settimana]
                    value.push(x)
                    week[x.settimana] = value
                }
            })
        }
    }

    year.append("g")
        .selectAll("rect")
        .data(() => Object.keys(week).map(key => week[key].map(numberWeek => numberWeek.settimana).map((e, i) => [e, index[i]])).flat())
        .join("rect")
        .attr("width", 50 + cellSize - 1)
        .attr("height", cellSize - 1)
        .attr("x", d => (d[0] + d[0] + d[0] + d[0] - 4) * cellSize + 0.5)
        .attr("y", d => countDay(d[1]) * cellSize + 0.5)
        .attr("fill", d => {
            console.log(color()(d[1]))
            return color()(d[1])
        })
        .append("title")
        .text(d => 1);


    const month = year.append("g")
        .selectAll("g")
        .data([1, 2, 3, 4])
        .join("g");
    const multi = [1, 5, 9, 13]
    month.filter((d, i) => i).append("path")
        .attr("fill", "none")
        .attr("stroke", "#fff")
        .attr("stroke-width", 3)
        .attr("d", [1, 2, 3, 4]);

    month.append("text")
        .attr("position", "center")
        .attr("x", d => multi[d - 1] * cellSize + 12)
        .attr("y", -3)
        .text(x => x); 
} 