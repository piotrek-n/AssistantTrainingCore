import React, { useState, useEffect, useReducer } from 'react';
import { toDataSourceRequestString, translateDataSourceResultGroups } from '@progress/kendo-data-query';
import {getIncompleteTraining, getInstructionsWithoutTraining, getProducts, getWorkersWithoutTraining } from './api';
import {IncompleteTrainingDataReport, InstructionsWithoutTrainingResult, Product, WorkersWithoutTrainingResult } from './models';
import axios from 'axios';

import { Grid, GridColumn } from "@progress/kendo-react-grid";
//import { process, State } from "@progress/kendo-data-query";

import { DropDownList, DropDownListChangeEvent } from "@progress/kendo-react-dropdowns";




import products from "./products.json";
import ReportGridChild from './GridChild';
import { Button } from '@progress/kendo-react-buttons';



const ReportGrid = () => {

    const reports = [
        { text: "Wybierz", id: 1 },
        { text: "SZKOLENIA", id: 2 },
        { text: "INSTRUKCJE", id: 3 },
        { text: "PRACOWNICY", id: 4 },
    ]

    const [selectedReport, setSelectedReport] = React.useState({
        value: { text: "Wyczyść", id: 1 },
    });

    const [dataState, setDataState] = useState({ skip: 0, take: 20 });
    const [total, setTotal] = useState(0);

    const [dataGrid, setDataGrid] = useState<Product[] | []>([]);
    const [dataAllColumns, setDataAllColumns] = useState<JSX.Element[] | []>([]);

    const [dataIncompleteTrainingGrid, setIncompleteTrainingDataGrid] = useState<IncompleteTrainingDataReport[] | []>([]);
    const [dataWorkersWithoutTrainingGrid, setWorkersWithoutTrainingDataGrid] = useState<WorkersWithoutTrainingResult[] | []>([]);
    const [dataInstructionsWithoutTrainingGrid, setInstructionsWithoutTrainingDataGrid] = useState<InstructionsWithoutTrainingResult[] | []>([]);


    //useEffect(() => {

    //    getProducts(`${toDataSourceRequestString(dataState)}`)
    //        .then(products => {
    //            console.log(products);

    //            setTimeout(function () {
    //            var allColumns = products.Data.length > 0 ? Object.keys(products.Data[0]) : []
    //            var columnsToShow = allColumns.map((item, i) => <GridColumn field={item} key={i} />);
    //                setDataAllColumns(columnsToShow);
    //            }, 1000);

    //            setTotal(products.Total);
    //            setDataGrid(products.Data);

    //        });

    //}, []);
    //const [_, forceUpdate] = useReducer((x) => x + 1, 0);
    //var test = dataAllColumns.map((item, i) => {

    //    return (
    //        <GridColumn
    //            field={item} key={i} />
    //    );
    //});

    var allColumns = products.length > 0 ? Object.keys(products[0]) : []
    var columnsToShow = allColumns.map((item, i) => <GridColumn width="100px" field={item} key={i} />);

    const handleChange = (event: DropDownListChangeEvent) => {

        setTotal(0);
        setDataGrid([]);

        setSelectedReport({
            value: event.target.value,
        });

        switch (event.target.value.id)
        {
            case 2:
                getIncompleteTraining(`${toDataSourceRequestString(dataState)}`, event.target.value.id)
                    .then(incompleteTraining => {
                        console.log(incompleteTraining);

                        // setTimeout(function () {
                        //     var allColumns = products.Data.length > 0 ? Object.keys(products.Data[0]) : []
                        //     var columnsToShow = allColumns.map((item, i) => <GridColumn field={item} key={i} />);
                        //     setDataAllColumns(columnsToShow);
                        // }, 1000);

                        setTotal(incompleteTraining.Total);
                        setIncompleteTrainingDataGrid(incompleteTraining.Data);

                    });
                break;
            case 3:
                getWorkersWithoutTraining(`${toDataSourceRequestString(dataState)}`, event.target.value.id)
                    .then(workersWithoutTraining => {
                        console.log(workersWithoutTraining);

                        // setTimeout(function () {
                        //     var allColumns = products.Data.length > 0 ? Object.keys(products.Data[0]) : []
                        //     var columnsToShow = allColumns.map((item, i) => <GridColumn field={item} key={i} />);
                        //     setDataAllColumns(columnsToShow);
                        // }, 1000);

                        setTotal(workersWithoutTraining.Total);
                        setWorkersWithoutTrainingDataGrid(workersWithoutTraining.Data);

                    });
                break;
            case 4:
                getInstructionsWithoutTraining(`${toDataSourceRequestString(dataState)}`, event.target.value.id)
                    .then(instructionsWithoutTraining => {
                        console.log(instructionsWithoutTraining);

                        // setTimeout(function () {
                        //     var allColumns = products.Data.length > 0 ? Object.keys(products.Data[0]) : []
                        //     var columnsToShow = allColumns.map((item, i) => <GridColumn field={item} key={i} />);
                        //     setDataAllColumns(columnsToShow);
                        // }, 1000);

                        setTotal(instructionsWithoutTraining.Total);
                        setInstructionsWithoutTrainingDataGrid(instructionsWithoutTraining.Data);

                    });
                break;
        }
        // useEffect
        getProducts(`${toDataSourceRequestString(dataState)}`, event.target.value.id)
            .then(products => {
                console.log(products);

                setTimeout(function () {
                    var allColumns = products.Data.length > 0 ? Object.keys(products.Data[0]) : []
                    var columnsToShow = allColumns.map((item, i) => <GridColumn field={item} key={i} />);
                    setDataAllColumns(columnsToShow);
                }, 1000);

                setTotal(products.Total);
                setDataGrid(products.Data);

            });
    };
    
    return (<>
        <div>
            <div className="example-config">
                Wybrano raport: {JSON.stringify(selectedReport.value.text)}
            </div>
            <DropDownList
                style={{ width: "300px" }}
                data={reports}
                textField="text"
                dataItemKey="id"
                value={selectedReport.value}
                onChange={handleChange}
            />
            <form action="/spreadprocessing/generatedocument" method="post" style={{ display:"inline"}}>
                <input type="hidden"  value={selectedReport.value.text} name="fileFormat" />
                <input type="submit" className="k-button k-primary wide-btn" value="Generate and Download" />
            </form>
        </div>
            

        {selectedReport.value.id > 1 && dataGrid.length > 0 && <Grid 
            data={dataGrid} 
            total={total}
            sortable={true}
            pageable={{ pageSizes: true }}
        >
            {dataAllColumns}
        </Grid>}

            {selectedReport.value.id == 2 && dataIncompleteTrainingGrid.length > 0 && 
                <Grid
                data={dataIncompleteTrainingGrid}
                total={total}
                sortable={true}
                pageable={{ pageSizes: true }}
            >
                <GridColumn field="TrainingNumber" title="Training Number" />
                <GridColumn field="InstructionNumber" title="Instruction Number"  />
            </Grid>}     


    </>
    );

};

export default ReportGrid;

