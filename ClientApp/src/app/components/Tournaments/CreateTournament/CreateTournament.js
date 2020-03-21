import React, {useCallback, useState} from 'react';
import Stepper from '@material-ui/core/Stepper';
import Step from '@material-ui/core/Step';
import StepLabel from '@material-ui/core/StepLabel';
import StepContent from '@material-ui/core/StepContent';
import Button from '@material-ui/core/Button';
import Paper from '@material-ui/core/Paper';
import Typography from '@material-ui/core/Typography';
import {Formik, Form, Field} from 'formik';
import {useStyles} from "./CreateTournamentStyle";
import {newTournamentValidation} from "../../../utils/validation";
import {Helmet} from "react-helmet";
import {TournamentsCreate, TournamentsMain} from "../utils";
import {Canonicals} from "../../../utils/helpers";
import BasicTournamentInfo from "./BasicTournamentInfo";
import _ from "lodash";
import TournamentType from "./TournamentType";

const initialValues = {
    tournamentIcon: null,
    tournamentTitle: '',
    tournamentDescription: '',
    tournamentType: ''
};

function getSteps() {
    return ['Create basic tournament info', 'Customize tournament type', 'Invite friends'];
}

/**
 * @return {null}
 */
export default function CreateTournament() {
    if (!process.env.NODE_ENV || process.env.NODE_ENV === 'production') {
        return null;
    }
    
    const classes = useStyles();
    const [activeStep, setActiveStep] = React.useState(0);
    const steps = getSteps();

    const handleNext = () => {
        setActiveStep(prevActiveStep => prevActiveStep + 1);
    };

    const handleBack = () => {
        setActiveStep(prevActiveStep => prevActiveStep - 1);
    };

    const handleReset = () => {
        setActiveStep(0);
    };

    function getStepContent(step, formProps) {
        switch (step) {
            case 0:
                return <BasicTournamentInfo formProps={formProps} />;
            case 1:
                return <TournamentType formProps={formProps} />;
            case 2:
                return `Try out different ad text to see what brings in the most customers,
              and learn how to enhance your ads using features like ad extensions.
              If you run into any problems with your ads, find out how to tell if
              they're running and how to resolve approval issues.`;
            default:
                return 'Unknown step';
        }
    }
    
    function handleCanContinue(step, formProps) {
        const {touched, errors} = formProps;
        switch (step) {
            case 0:
                const isIconSelected = _.isEmpty(errors.tournamentIcon);
                const isTitleEntered = touched.tournamentTitle && _.isEmpty(errors.tournamentTitle);
                const isDescriptionEntered = touched.tournamentDescription && _.isEmpty(errors.tournamentDescription);
                return isIconSelected && isTitleEntered && isDescriptionEntered;
            case 1:
                return isIconSelected && isTitleEntered && isDescriptionEntered;
            case 2:
                return isIconSelected && isTitleEntered && isDescriptionEntered;
            default:
                return isIconSelected && isTitleEntered && isDescriptionEntered;
        }
    }
    return (
        <>
            <Helmet>
                <title>Create New Tournament | Fantasy Hoops</title>
                <meta name="description" content={TournamentsCreate.SUBTITLE}/>
                <link rel="canonical" href={Canonicals.TOURNAMENTS}/>
            </Helmet>
            <article className="Tournaments__Intro">
                <h1 className="Tournaments__Title">{TournamentsCreate.TITLE}</h1>
                <h5 className="Tournaments__Subtitle">{TournamentsCreate.SUBTITLE}</h5>
            </article>
            <Formik
                initialValues={initialValues}
                validationSchema={newTournamentValidation}
                onSubmit={(values, actions) => {
                    actions.setSubmitting(true);
                    const preview = document.getElementsByClassName('markdown-preview')[0];
                    preview.innerHTML = null;
                    const {savePost} = this.props;
                    savePost({title: values.title, body: values.body, authorID: author.id});
                    actions.resetForm({});
                    actions.setSubmitting(false);
                }}
                render={(formProps) => {
                    const {values} = formProps;
                    const canContinue = handleCanContinue(activeStep, formProps);
                    return (
                        <div className={classes.root}>
                            <Stepper activeStep={activeStep} orientation="vertical">
                                {steps.map((label, index) => (
                                    <Step key={label}>
                                        <StepLabel>{label}</StepLabel>
                                        <StepContent>
                                            <Typography variant="subtitle1">{getStepContent(index, formProps)}</Typography>
                                            <div className={classes.actionsContainer}>
                                                <div>
                                                    <Button
                                                        disabled={activeStep === 0}
                                                        onClick={handleBack}
                                                        className={classes.button}
                                                    >
                                                        Back
                                                    </Button>
                                                    <Button
                                                        variant="contained"
                                                        color="primary"
                                                        onClick={handleNext}
                                                        className={classes.button}
                                                        disabled={!canContinue}
                                                    >
                                                        {activeStep === steps.length - 1 ? 'Finish' : 'Next'}
                                                    </Button>
                                                </div>
                                            </div>
                                        </StepContent>
                                    </Step>
                                ))}
                            </Stepper>
                            {activeStep === steps.length && (
                                <Paper square elevation={0} className={classes.resetContainer}>
                                    <Typography>All steps completed - you&apos;re finished</Typography>
                                    <Button onClick={handleReset} className={classes.button}>
                                        Reset
                                    </Button>
                                </Paper>
                            )}
                        </div>
                    )
                }}/>
        </>
    );
}