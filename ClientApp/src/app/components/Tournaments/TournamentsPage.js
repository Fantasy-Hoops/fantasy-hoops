import React from "react";
import {Helmet} from "react-helmet";
import {TournamentsMain} from "./utils";
import {Canonicals} from "../../utils/helpers";

import './TournamentsPage.css';

/**
 * @return {null}
 */
export default function TournamentsPage() {
    if (!process.env.NODE_ENV || process.env.NODE_ENV === 'production') {
        return null;
    }
    
    return (
        <>
            <Helmet>
                <title>Tournaments | Fantasy Hoops</title>
                <meta name="description" content={TournamentsMain.SUBTITLE}/>
                <link rel="canonical" href={Canonicals.TOURNAMENTS}/>
            </Helmet>
            <article className="Tournaments__Intro">
                <h1 className="Tournaments__Title">{TournamentsMain.TITLE}</h1>
                <h5 className="Tournaments__Subtitle">{TournamentsMain.SUBTITLE}</h5>
            </article>
        </>
    );
}