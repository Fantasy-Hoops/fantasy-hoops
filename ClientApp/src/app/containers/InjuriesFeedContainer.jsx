import React, {useEffect} from 'react';
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

const Intro = {
    TITLE: "INJURIES",
    SUBTITLE: "Latest NBA information concerning player injuries, illnesses and rest for all NBA games. " +
        "Injuries report in Fantasy Hoops updates every 10 minutes so you will never miss any changes that could affect your lineup. " +
        "Click on cards to flip and reveal detailed information about player status."
};

const mapStateToProps = state => ({
    injuries: state.injuriesContainerReducer.injuries,
    injuryLoader: state.injuriesContainerReducer.injuryLoader
});

const mapDispatchToProps = dispatch => bindActionCreators(actionCreators, dispatch);

function InjuriesFeedContainer(props) {
    let content;
    const {injuries, injuryLoader, loadInjuries} = props;
    const [open, setOpen] = React.useState(false);
    const classes = useStyles();

    const handleClickOpen = () => {
        setOpen(true);
    };
    const handleClose = () => {
        setOpen(false);
    };

    function transition() {
        if (this.classList.contains('active')) {
            this.lastChild.classList.add('overflow-hidden');
            this.lastChild.classList.remove('overflow-auto');
            this.classList.remove('active');
        } else if (this.lastChild.lastChild.lastChild.innerHTML !== '') {
            this.classList.add('active');
            this.lastChild.classList.add('overflow-auto');
            this.lastChild.classList.remove('overflow-hidden');
        }
    }

    useEffect(() => {
        async function handleLoadInjuries() {
            await loadInjuries();
        }

        handleLoadInjuries();
    }, []);

    useEffect(() => {
        const cards = document.querySelectorAll('.InjuryCard');

        cards.forEach(card => card.addEventListener('click', transition));
    });

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

    const injuryCards = _.map(injuries, (injury, index) => {
        const animated = index === 0 ? 'animated pulse delay-2s' : '';
        return <InjuryCard key={shortid()} injury={injury} animated={animated}/>;
    });
    return (
        <>
            <Helmet>
                <title>Injuries | Fantasy Hoops</title>
                <meta name="description" content={Intro.SUBTITLE} />
                <link rel="canonical" href={Canonicals.INJURIES} />
            </Helmet>
            <Container maxWidth="md">
                <article className="Injuries__Intro">
                    <h1 className="Injuries__Title">{Intro.TITLE}</h1>
                    <h5 className="Injuries__Subtitle">{Intro.SUBTITLE}</h5>
                </article>
                <Button className={classes.button} color="primary" onClick={handleClickOpen}>
                    INFO
                </Button>
                <div className="InjuryContainer__Cards">{content || injuryCards}</div>
                <InjuriesInfoDialog handleClose={handleClose} open={open}/>
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
