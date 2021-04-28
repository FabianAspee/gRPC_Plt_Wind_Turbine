window.setup = (id, config) => { 
    var ctx = document.getElementById(id).getContext('2d');
    new Chart(ctx, config);
}
window.setupAreaChart = (id, config) => {
    var ctx = document.getElementById(id).getContext('2d');
    new Chart(ctx, config);
}
window.setupLineChart = (id, config) => {
    var ctx = document.getElementById(id).getContext('2d');
    new Chart(ctx, config);
}
window.setupScatterChart = (id, config) => {
    var ctx = document.getElementById(id).getContext('2d');
    config.options.scales = {
        xAxes: [{
            ticks: {
                userCallback: function (label, index, labels) {
                    return moment(label).format("DD/MM/YY");
                }
            }
        }]
    }

    new Chart(ctx, config);
}

window.setupscatter = (id, config) => {

    var ctx = document.getElementById(id).getContext('2d');
    config.options.scales =  {
        xAxes: [{
            ticks: {
                userCallback: function (label, index, labels) {
                    return moment(label).format("DD/MM/YY");
                }
            }
        }]
    }

    new Chart(ctx, config);
}
 

window.setupspecialline = (id, config) => {
    console.log(config)
    const skipped = (ctx, value) => ctx.p0.skip || ctx.p1.skip ? value : undefined;
    const down = (ctx, value) => ctx.p0.parsed.y > ctx.p1.parsed.y ? value : undefined;
    var ctx = document.getElementById(id).getContext('2d');
   
    var segment = {
        borderColor: ctx => skipped(ctx, 'rgb(0,0,0,0.2)') || down(ctx, 'rgb(192,75,75)'),
        borderDash: ctx => skipped(ctx, [6, 6]),
    }
    config.data.datasets[0].segment = segment 

    new Chart(ctx, config);
}

window.setupspeciallines = (id) => {
    const genericOptions = {
        fill: false,
        interaction: {
            intersect: false
        },
        radius: 0,
    };
    var ctx = document.getElementById(id).getContext('2d');
    const skipped = (ctx, value) => ctx.p0.skip || ctx.p1.skip ? value : undefined;
    const down = (ctx, value) => ctx.p0.parsed.y > ctx.p1.parsed.y ? value : undefined;
    const config = {
        type: 'line',
        data: {
            labels: ['January', 'February', "March", "April", "May", "June", "July"],
            datasets: [{
                label: 'My First Dataset',
                data: [65, 59, NaN, 48, 56, 57, 40],
                borderColor: 'rgb(75, 192, 192)',
                segment: {
                    borderColor: ctx => skipped(ctx, 'orange') || down(ctx, 'rgb(192,75,75)'),
                    borderDash: ctx => skipped(ctx, [6, 6]),
                }
            }]
        },
        options: genericOptions
    };

    new Chart(ctx, config);
}
