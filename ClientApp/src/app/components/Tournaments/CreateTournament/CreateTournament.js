import React, {useCallback, useEffect, useState} from 'react';
import Stepper from '@material-ui/core/Stepper';
import Step from '@material-ui/core/Step';
import StepLabel from '@material-ui/core/StepLabel';
import StepContent from '@material-ui/core/StepContent';
import Button from '@material-ui/core/Button';
import Typography from '@material-ui/core/Typography';
import {Formik} from 'formik';
import {useStyles} from "./CreateTournamentStyle";
import {newTournamentValidation} from "../../../utils/validation";
import {Helmet} from "react-helmet";
import {TournamentsCreate} from "../utils";
import {Canonicals, TOURNAMENT_DATE_FORMAT} from "../../../utils/helpers";
import BasicTournamentInfo from "./BasicTournamentInfo";
import _ from "lodash";
import TournamentType from "./TournamentType";
import {Error} from "../../Error";
import InviteFriends from "./InviteFriends";
import {
    createTournament,
    getTournamentStartDates,
    getTournamentTypes,
    getUserFriends
} from "../../../utils/networkFunctions";
import {parse} from "../../../utils/auth";
import moment from "moment";
import {loadImage} from "../../../utils/loadImage";
import defaultPhoto from "../../../../content/images/default.png";
import FullscreenLoader from "../../FullscreenLoader";
import TournamentSummary from "./TournamentSummary";
import {useSnackbar} from "notistack";
import CopyToClipboard from "../../Inputs/CopyToClipboard";

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


/**
 * @return {null}
 */

export default function CreateTournament() {
    const user = parse();
    const {enqueueSnackbar} = useSnackbar();
    const [tournamentTypes, setTournamentTypes] = useState([]);
    const [startDates, setStartDates] = useState([]);
    const [initialContests, setInitialContests] = useState([]);
    const [initialDroppedContests, setInitialDroppedContests] = useState([]);
    const [userFriends, setUserFriends] = useState([]);
    const [typesLoader, setTypesLoader] = useState(false);
    const [datesLoader, setDatesLoader] = useState(false);
    const [friendsLoader, setFriendsLoader] = useState(false);
    const [submitLoader, setSubmitLoader] = useState(false);
    const [inviteUrl, setInviteUrl] = useState(null);

    useEffect(() => {
        async function handleGetTournamentTypes() {
            setTypesLoader(true);
            await getTournamentTypes().then(response => {
                const tournamentTypes = response.data.map(tournament => ({
                    value: tournament.id,
                    label: tournament.name
                }));
                setTournamentTypes(tournamentTypes);
            }).catch(error => {
                enqueueSnackbar(error.message, {variant: 'error'});
            });
            setTypesLoader(false);
        }

        async function handleGetUserFriends() {
            setFriendsLoader(true);

            async function loadUserImage(avatarURL) {
                return await loadImage(`${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/avatars/${avatarURL}.png`, defaultPhoto);
            }

            await getUserFriends(user.id)
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
                        .then(data => {
                            setUserFriends(data);
                        })
                        .catch(err => enqueueSnackbar(err.message, {variant: 'error'}));

                }).catch(error => {
                    enqueueSnackbar(error.message, {variant: 'error'});
                });
            setFriendsLoader(false);
        }

        async function handleGetTournamentStartDates() {
            setDatesLoader(true);
            await getTournamentStartDates().then(response => {
                const startDates = response.data.map((date, index) => ({
                    index: index,
                    value: date,
                    label: moment(date).format(TOURNAMENT_DATE_FORMAT)
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
            }).catch(error => {
                console.error(error.message);
            });
            setDatesLoader(false);
        }

        handleGetTournamentTypes();
        handleGetUserFriends();
        handleGetTournamentStartDates();
    }, []);

    const classes = useStyles();
    const [activeStep, setActiveStep] = React.useState(0);
    const steps = getSteps();

    const handleNext = (values) => {
        localStorage.setItem('tournamentValues', JSON.stringify(values));
        setActiveStep(prevActiveStep => prevActiveStep + 1);
    };

    const handleBack = () => {
        setActiveStep(prevActiveStep => prevActiveStep - 1);
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
        const {values, touched, errors} = formProps;
        switch (step) {
            case 0:
                const isIconSelected = _.isEmpty(errors.tournamentIcon);
                const isTitleEntered = (values.tournamentTitle !== '' || touched.tournamentTitle) && _.isEmpty(errors.tournamentTitle);
                const isDescriptionEntered = (values.tournamentDescription !== '' || touched.tournamentDescription) && _.isEmpty(errors.tournamentDescription);
                return isIconSelected && isTitleEntered && isDescriptionEntered;
            case 1:
                const isTypeSelected = (values.tournamentType !== '' || touched.tournamentType) && _.isEmpty(errors.tournamentType);
                const isStartDateSelected = (values.startDate !== '' || touched.startDate) && _.isEmpty(errors.startDate);
                const isContestsSelected = (values.contests !== '' || touched.contests) && _.isEmpty(errors.contests);
                const isDroppedContestsSelected = (values.droppedContests !== '' || touched.droppedContests) && _.isEmpty(errors.droppedContests);
                const isEntrantsSelected = (values.entrants !== '' || touched.entrants) && _.isEmpty(errors.entrants);
                return isTypeSelected && isStartDateSelected && isContestsSelected
                    && isDroppedContestsSelected && isEntrantsSelected;
            case 2:
                return true;
            default:
                return false;
        }
    }

    const showLoader = typesLoader || datesLoader || friendsLoader || submitLoader;


    if (!process.env.NODE_ENV || process.env.NODE_ENV === 'production') {
        return <Error status={404}/>;
    }

    if (inviteUrl) {
        return (
            <>
                <Helmet>
                    <title>Create New Tournament | Fantasy Hoops</title>
                    <meta name="description" content={TournamentsCreate.SUBTITLE}/>
                    <link rel="canonical" href={Canonicals.TOURNAMENTS}/>
                </Helmet>
                <article className="Tournaments__Intro">
                    <h1 className="Tournaments__Title">{TournamentsCreate.TITLE}</h1>
                    <h1 className="Tournaments__Subtitle">{TournamentsCreate.CREATED_SUBTITLE}</h1>
                </article>
                <CopyToClipboard inputText={inviteUrl} />
            </>
        )
    }

    return (
        <>
            <Helmet>
                <title>Create New Tournament | Fantasy Hoops</title>
                <meta name="description" content={TournamentsCreate.SUBTITLE}/>
                <link rel="canonical" href={Canonicals.TOURNAMENTS_CREATE}/>
            </Helmet>
            <article className="Tournaments__Intro">
                <h1 className="Tournaments__Title">{TournamentsCreate.TITLE}</h1>
            </article>
            <Formik
                initialValues={JSON.parse(localStorage.getItem('tournamentValues')) || initialValues}
                validationSchema={newTournamentValidation}
                onSubmit={(values, actions) => {
                    actions.setSubmitting(true);
                    setSubmitLoader(true);
                    createTournament({
                        creatorId: user.id,
                        tournamentIcon: values.tournamentIcon,
                        tournamentTitle: values.tournamentTitle,
                        tournamentDescription: values.tournamentDescription,
                        startDate: values.startDate,
                        tournamentType: values.tournamentType,
                        contests: values.contests,
                        droppedContests: values.droppedContests,
                        entrants: values.entrants,
                        userFriends: values.userFriends.map(user => user.id)
                    })
                        .then(response => {
                            enqueueSnackbar(response.data.message, {variant: 'success'});
                            setInviteUrl(response.data.inviteUrl);
                            localStorage.removeItem('tournamentValues');
                            setSubmitLoader(false);
                        })
                        .catch(error => {
                            enqueueSnackbar(`${error.message}\n${error.response.data}`, {variant: 'error'});
                            setSubmitLoader(false);
                        });
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
                                                        onClick={() => handleNext(values)}
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
                                <TournamentSummary formProps={formProps} handleBack={handleBack}/>
                            )}
                        </div>
                    )
                }}
            />
            {showLoader && <FullscreenLoader/>}
        </>
    );
}