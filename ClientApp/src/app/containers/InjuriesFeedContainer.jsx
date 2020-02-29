import React, {useEffect, useState} from 'react';
import {Helmet} from 'react-helmet';
import PropTypes from 'prop-types';
import {connect} from 'react-redux';
import {bindActionCreators} from 'redux';
import _ from 'lodash';
import shortid from 'shortid';
import InjuryCard from '../components/Injuries/InjuryCard';
import EmptyJordan from '../components/EmptyJordan';
import * as actionCreators from '../actions/injuries';

import './InjuriesFeedContainer.css';
import {Container} from "@material-ui/core";
import InjuriesInfoDialog from "../components/Injuries/InjuriesInfoDialog";
import Button from "@material-ui/core/Button";
import {useStyles} from "./InjuriesFeedContainerStyle";
import {Canonicals} from "../utils/helpers";
import InjuryPlayerDialog from "../components/Injuries/InjuryPlayerDialog";

const Intro = {
    TITLE: "INJURIES",
    SUBTITLE: "Latest NBA information concerning player injuries, illnesses and rest for all NBA games. " +
        "Injuries report in Fantasy Hoops updates every 10 minutes so you will never miss any changes that could affect your lineup. " +
        "Click on cards to flip and reveal detailed information about player status."
};

const googleAd = (
    <>
        <script async src="https://pagead2.googlesyndication.com/pagead/js/adsbygoogle.js"></script>
        <ins className="adsbygoogle"
             style={{display: "block"}}
             data-ad-client="ca-pub-6391166063453559"
             data-ad-slot="7873415467"
             data-ad-format="auto"
             data-full-width-responsive="true"></ins>
        <script>
            (adsbygoogle = window.adsbygoogle || []).push({});
        </script>
    </>
);

const mapStateToProps = state => ({
    injuries: state.injuriesContainerReducer.injuries,
    injuryLoader: state.injuriesContainerReducer.injuryLoader
});

const mapDispatchToProps = dispatch => bindActionCreators(actionCreators, dispatch);

function InjuriesFeedContainer(props) {
    let content;
    const {injuries, injuryLoader, loadInjuries} = props;
    const [open, setOpen] = React.useState(false);
    const [dialogInjury, setDialogInjury] = useState(null);
    const [injuryPlayerDialogOpen, setInjuryPlayerDialogOpen] = React.useState(false);
    const classes = useStyles();

    const handleClickOpen = () => {
        setOpen(true);
    };
    const handleClose = () => {
        setOpen(false);
    };

    const handleInjuryPlayerDialogClose = () => {
        setInjuryPlayerDialogOpen(false);
    };
    const handleInjuryPlayerDialogOpen = injury => {
        setDialogInjury(injury);
        setInjuryPlayerDialogOpen(true);
    };

    useEffect(() => {
        async function handleLoadInjuries() {
            await loadInjuries();
        }

        handleLoadInjuries();
    }, []);

    if (injuryLoader) {
        content = (
            <div className="my-5 mx-auto">
                <div className="Loader"/>
            </div>
        );
    }
    if (!injuryLoader && injuries && injuries.length === 0) {
        content = (
            <div className="my-5 mx-auto">
                <EmptyJordan message="No injuries report today..."/>
            </div>
        );
    }

    const injuryCards = _.map(injuries, (injury) => {
        return <InjuryCard key={shortid()} injury={injury} handleOpenDialog={handleInjuryPlayerDialogOpen}/>;
    });
    return (
        <>
            <Helmet>
                <title>Injuries | Fantasy Hoops</title>
                <meta name="description" content={Intro.SUBTITLE}/>
                <link rel="canonical" href={Canonicals.INJURIES}/>
            </Helmet>
            <Container maxWidth="md">
                <article className="Injuries__Intro">
                    <h1 className="Injuries__Title">{Intro.TITLE}</h1>
                    <h5 className="Injuries__Subtitle">{Intro.SUBTITLE}</h5>
                </article>
                {googleAd}
                <Button className={classes.button} color="primary" onClick={handleClickOpen}>
                    INFO
                </Button>
                <div className="InjuryContainer__Cards">{injuryCards.length > 0 ? injuryCards : content}</div>
                <InjuriesInfoDialog handleClose={handleClose} open={open}/>
                <InjuryPlayerDialog handleDialogClose={handleInjuryPlayerDialogClose} open={injuryPlayerDialogOpen} injury={dialogInjury}/>
            </Container>
        </>
    );
}

InjuriesFeedContainer.propTypes = {
    loadInjuries: PropTypes.func.isRequired,
    injuries: PropTypes.arrayOf(
        PropTypes.shape({
            injuryID: PropTypes.number.isRequired
        })
    ).isRequired,
    injuryLoader: PropTypes.bool.isRequired
};

export default connect(mapStateToProps, mapDispatchToProps)(InjuriesFeedContainer);
