import React, { useState, useEffect } from 'react';
import { Product } from './models';
import axios from 'axios';

import { Grid, GridColumn } from "@progress/kendo-react-grid";

import productsFromFile from "./products.json";

type ReportGridChildProps = {
    title: string;
    children?: JSX.Element | JSX.Element[];
    products: Product[] | [];
};

const ReportGridChild = ({ title, children, products }: ReportGridChildProps) => {
    console.log("productsFromFile:");
    console.log(productsFromFile);
    console.log("products:");
    console.log(products);
    return (<div>
        {
            
        }
        {title}
        <Grid data={productsFromFile}>
            {children}
        </Grid>
    </div>
    );
};

export default ReportGridChild;