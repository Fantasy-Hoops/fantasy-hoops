import React, {useCallback, useEffect, useState} from 'react';
import Stepper from '@material-ui/core/Stepper';
import Step from '@material-ui/core/Step';
import StepLabel from '@material-ui/core/StepLabel';
import StepContent from '@material-ui/core/StepContent';
import Button from '@material-ui/core/Button';
import Paper from '@material-ui/core/Paper';
import Typography from '@material-ui/core/Typography';
import {Formik, Form, Field, ErrorMessage} from 'formik';
import {useStyles} from "./CreateTournamentStyle";
import {newTournamentValidation} from "../../../utils/validation";
import {Helmet} from "react-helmet";
import {TournamentsCreate, TournamentsMain} from "../utils";
import {Canonicals} from "../../../utils/helpers";
import BasicTournamentInfo from "./BasicTournamentInfo";
import _ from "lodash";
import TournamentType from "./TournamentType";
import {Error} from "../../Error";
import InviteFriends from "./InviteFriends";
import {getTournamentStartDates, getTournamentTypes, getUserFriends} from "../../../utils/networkFunctions";
import {parse} from "../../../utils/auth";
import moment from "moment";
import {loadImage} from "../../../utils/loadImage";
import defaultPhoto from "../../../../content/images/default.png";

const DATE_FORMAT = 'MMMM Do YYYY, h:mm:ss a';

const initialValues = {
    tournamentIcon: null,
    tournamentTitle: '',
    tournamentDescription: '',
    startDate: '',
    tournamentType: '',
    contests: '',
    droppedContests: '',
    entrants: '',
    userFriends: []
};

function getSteps() {
    return ['Create basic tournament info', 'Customize tournament type', 'Invite friends'];
}

export default function CreateTournament() {
    if (!process.env.NODE_ENV || process.env.NODE_ENV === 'production') {
        return <Error status={404}/>;
    }

    const [tournamentTypes, setTournamentTypes] = useState([]);
    const [startDates, setStartDates] = useState([]);
    const [initialContests, setInitialContests] = useState([]);
    const [initialDroppedContests, setInitialDroppedContests] = useState([]);
    const [userFriends, setUserFriends] = useState([]);

    useEffect(() => {
        async function handleGetTournamentTypes() {
            await getTournamentTypes().then(response => {
                const tournamentTypes = response.data.map(tournament => ({
                    value: tournament.id,
                    label: tournament.name
                }));
                setTournamentTypes(tournamentTypes);
            }).catch(error => console.error(error.message));
        }

        async function handleGetUserFriends() {
            async function loadUserImage(avatarURL) {
                return await loadImage(`${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/avatars/${avatarURL}.png`, defaultPhoto);
            }
            
            await getUserFriends(parse().id)
                .then(response => {
                    const friends = response.data.map(async friend => {
                        const avatar = await loadUserImage(friend.avatarURL);
                        return {
                            id: friend.id,
                            userName: friend.userName,
                            imageSrc: avatar
                        };
                    });
                    Promise.all(friends)
                        .then(data => setUserFriends(data))
                        .catch(err => console.error(err.message));
                }).catch(error => console.error(error.message));
        }

        async function handleGetTournamentStartDates() {
            await getTournamentStartDates().then(response => {
                const startDates = response.data.map((date, index) => ({
                    index: index,
                    value: moment(date),
                    label: moment(date).format(DATE_FORMAT)
                }));
                const contests = response.data.map((date, index) => ({
                    index: index,
                    value: index + 1,
                    label: `${index + 1} Contests`
                }));
                const droppedContests = contests.slice(0).map((contest, index) => ({
                    index: index,
                    value: index,
                    label: `${index} Dropped Contests`
                }));
                setStartDates(startDates);
                setInitialContests(contests);
                setInitialDroppedContests(droppedContests);
            }).catch(error => console.error(error.message));
        }

        handleGetTournamentTypes();
        handleGetTournamentStartDates();
        handleGetUserFriends();
    }, []);

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
                return <BasicTournamentInfo formProps={formProps}/>;
            case 1:
                return (
                    <TournamentType
                        tournamentTypes={tournamentTypes}
                        startDates={startDates}
                        initialContests={initialContests}
                        initialDroppedContests={initialDroppedContests}
                        formProps={formProps}
                    />
                );
            case 2:
                return <InviteFriends userFriends={userFriends} formProps={formProps}/>;
            default:
                return 'Unknown step';
        }
    }

    function handleCanContinue(step, formProps) {
        return true;

        const {touched, errors} = formProps;
        switch (step) {
            case 0:
                const isIconSelected = _.isEmpty(errors.tournamentIcon);
                const isTitleEntered = touched.tournamentTitle && _.isEmpty(errors.tournamentTitle);
                const isDescriptionEntered = touched.tournamentDescription && _.isEmpty(errors.tournamentDescription);
                return isIconSelected && isTitleEntered && isDescriptionEntered;
            case 1:
                const isTypeSelected = touched.tournamentType && _.isEmpty(errors.tournamentType);
                const isStartDateSelected = touched.startDate && _.isEmpty(errors.startDate);
                const isContestsSelected = touched.contests && _.isEmpty(errors.contests);
                const isDroppedContestsSelected = touched.droppedContests && _.isEmpty(errors.droppedContests);
                const isEntrantsSelected = touched.entrants && _.isEmpty(errors.entrants);
                return isTypeSelected && isStartDateSelected && isContestsSelected
                    && isDroppedContestsSelected && isEntrantsSelected;
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
                    const {values, errors} = formProps;
                    const canContinue = handleCanContinue(activeStep, formProps);
                    return (
                        <div className={classes.root}>
                            <Stepper activeStep={activeStep} orientation="vertical">
                                {steps.map((label, index) => (
                                    <Step key={label}>
                                        <StepLabel>{label}</StepLabel>
                                        <StepContent>
                                            <Typography
                                                variant="subtitle1">{getStepContent(index, formProps)}</Typography>
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
                }}
            />
        </>
    );
}