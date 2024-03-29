import React, { useEffect, useState } from 'react';
import shortid from 'shortid';
import _ from 'lodash';
import moment from 'moment';
import { parse } from '../../../utils/auth';
import Card from './Card';
import leaderboardLogo from '../../../../content/icons/1021175-winning/svg/006-winner-5.svg';
import EmptyJordan from '../../EmptyJordan';
import { getUsersLeaderboard, getUserFriendsOnlyLeaderboard, getPlayerStats } from '../../../utils/networkFunctions';
import { getWeek } from '../../../utils/date';
import CustomDatePicker from "../../Inputs/DatePicker/CustomDatePicker";
import { datePickerStyles } from "./LeaderboardStyle";
import { DatePickerTypes, makeJSDateObject } from "../../Inputs/DatePicker/utils";
import endOfWeek from "date-fns/endOfWeek";
import enLocale from "date-fns/locale/en-GB";
import { Helmet } from "react-helmet";
import { Canonicals, Meta } from "../../../utils/helpers";
import PlayerDialog from "../../PlayerModal/PlayerDialog";

const LEADERBOARD_SUPPORT_START_DATE = '2019-02-24';
const loggedInUser = parse();
const LOAD_COUNT = 10;
const { $ } = window;

function Leaderboard(props) {
    const minDate = new Date(LEADERBOARD_SUPPORT_START_DATE);
    const maxDate = new Date();
    maxDate.setDate(maxDate.getDate() - 1);

    const initialShowButtonState = {
        daily: false,
        dailyFriends: false,
        weekly: false,
        weeklyFriends: false,
        monthly: false,
        monthlyFriends: false
    };
    const [activeTab, setActiveTab] = useState('daily');
    const [friendsOnly, setFriendsOnly] = useState(false);
    const [leaderboard, setLeaderboard] = useState({
        daily: [],
        weekly: [],
        monthly: [],
        dailyFriends: [],
        weeklyFriends: [],
        monthlyFriends: [],
        fromDate: []
    });
    const [loader, setLoader] = useState(true);
    const [loadMoreLoader, setLoadMoreLoader] = useState(false);
    const [stats, setStats] = useState('');
    const [showButton, setShowButton] = useState(initialShowButtonState);
    const [modalLoader, setModalLoader] = useState(true);
    const [renderChild, setRenderChild] = useState(false);
    const [date, setDate] = useState(maxDate);
    const [week, setWeek] = useState(new Date());
    const [weekNumber, setWeekNumber] = useState(getWeek(new Date()));
    const [dateFormat, setDateFormat] = useState(null);
    const [playerDialogOpen, setPlayerDialogOpen] = useState(false);


    const activeType = friendsOnly ? `${activeTab}Friends` : activeTab;
    const dailyUsers = createUsers(friendsOnly ? leaderboard.dailyFriends : leaderboard.daily, true);
    const weeklyUsers = createUsers(friendsOnly ? leaderboard.weeklyFriends : leaderboard.weekly);
    const monthlyUsers = createUsers(friendsOnly ? leaderboard.monthlyFriends : leaderboard.monthly);
    const fromDateUsers = createUsers(leaderboard.fromDate);
    const seeMoreButton = seeMoreBtn(activeType);

    useEffect(() => {
        async function handleGetUsersLeaderboard() {
            await getUsersLeaderboard({ type: 'daily', date: '' })
                .then((res) => {
                    setShowButton(prevState => ({ ...prevState, daily: res.data.length === LOAD_COUNT }));
                    setLeaderboard(prevState => ({ ...prevState, daily: res.data }));
                    setActiveTab('daily');
                    setLoader(false);
                });
        }

        handleGetUsersLeaderboard();
    }, []);


    async function onDateChange(date) {
        if (!date) {
            setDate(date);
            return;
        }
        
        setLoader(true);
        setLeaderboard(prevState => ({ ...prevState, daily: [], dailyFriends: [], fromDate: [] }));
        const dateFormat = moment(date).format('YYYYMMDD');
        const type = friendsOnly ? `${activeTab}Friends` : activeTab;

        const users = !friendsOnly
            ? await getUsersLeaderboard({ type: activeTab, date: dateFormat })
            : await getUserFriendsOnlyLeaderboard(loggedInUser.id, { type: activeTab, date: dateFormat });

        showButton[type] = users.data.length === LOAD_COUNT;
        setLeaderboard(prevState => ({ ...prevState, [type]: users.data }));
        setLoader(false);
        setDate(date);
        setDateFormat(dateFormat);
    }

    async function onWeekChange(date) {
        if (!date) {
            setWeek(date);
            return;
        }

        const weekNum = getWeek(date);
        if (weekNumber === weekNum) {
            return;
        }

        setLoader(true);
        setLeaderboard(prevState => ({ ...prevState, weekly: [], weeklyFriends: [] }));

        const type = activeTab + (friendsOnly ? 'Friends' : '');
        const users = !friendsOnly
            ? await getUsersLeaderboard({ type: 'weekly', weekNumber: weekNum, year: moment(date).year() })
            : await getUserFriendsOnlyLeaderboard(loggedInUser.id, {
                type: 'weekly',
                weekNumber: weekNum,
                year: moment(date).year()
            });

        const sunday = new Date(makeJSDateObject(moment(date).day(7)));
        const today = new Date();
        setWeek(today <= sunday ? today : sunday);
        setWeekNumber(weekNum);
        setLeaderboard(prevState => ({ ...prevState, [type]: users.data }));
        setLoader(false);
    }

    async function toggleFriendsOnly() {
        setFriendsOnly(!friendsOnly);
        const type = activeTab + (!friendsOnly ? 'Friends' : '');

        if (leaderboard[type].length === 0) {
            setLoader(true);

            const users = friendsOnly
                ? await getUsersLeaderboard({ type: activeTab, date: dateFormat, weekNumber })
                : await getUserFriendsOnlyLeaderboard(loggedInUser.id, { type: activeTab, date: dateFormat, weekNumber });

            setShowButton(prevState => ({ ...prevState, daily: users.data.length === LOAD_COUNT }));
            setLeaderboard(prevState => ({ ...prevState, [type]: users.data }));
            setLoader(false);
        }
    }

    async function switchTab(e) {
        const activeTabURL = e.target.id.split(/-/)[0];
        const type = friendsOnly ? `${activeTabURL}Friends` : activeTabURL;

        if (activeTab === activeTabURL) {
            return;
        }

        setActiveTab(activeTabURL);

        if (leaderboard[type].length === 0) {
            setLoader(true);

            const users = !friendsOnly
                ? await getUsersLeaderboard({ type: activeTabURL, date: dateFormat, weekNumber })
                : await getUserFriendsOnlyLeaderboard(loggedInUser.id, { type: activeTabURL, date: dateFormat, weekNumber });

            setShowButton(prevState => ({ ...prevState, daily: users.data.length === LOAD_COUNT }));
            setLeaderboard(prevState => ({ ...prevState, [type]: users.data }));
            setLoader(false);
        }
    }

    async function loadMore() {
        const type = friendsOnly ? `${activeTab}Friends` : activeTab;

        setLoadMoreLoader(true);

        const users = !friendsOnly
            ? await getUsersLeaderboard({
                type: activeTab, from: leaderboard[type].length, limit: LOAD_COUNT, date: dateFormat, weekNumber
            })
            : await getUserFriendsOnlyLeaderboard(loggedInUser.id, {
                type: activeTab, from: leaderboard[type].length, limit: LOAD_COUNT, date: dateFormat, weekNumber
            });

        setShowButton(prevState => ({ ...prevState, daily: users.data.length === LOAD_COUNT }));
        setLeaderboard(prevState => ({ ...prevState, [type]: leaderboard[type].concat(users.data) }));
        setLoader(false);
    }

    function seeMoreBtn(type) {
        return showButton[type] ?
            <button type="button" className="btn btn-primary mt-2" onClick={loadMore}>See more</button> : '';
    }

    function handlePlayerDialogOpen() {
        setPlayerDialogOpen(true);
    }

    function handlePlayerDialogClose() {
        setPlayerDialogOpen(false);
    }

    async function showModal(nbaID) {
        handlePlayerDialogOpen();
        await getPlayerStats(nbaID)
            .then((res) => {
                setStats(res.data);
                setModalLoader(false);
                setRenderChild(true);
            });
    }

    function createUsers(users, isDaily) {
        return _.map(
            users,
            (user, index) => (
                <Card
                    isDaily={isDaily}
                    index={index}
                    key={shortid()}
                    user={user}
                    showModal={showModal}
                />
            )
        );
    }

    return (
        <>
            <Helmet>
                <title>Users Leaderboard | Fantasy Hoops</title>
                <meta property="title" content="Users Leaderboard | Fantasy Hoops"/>
                <meta property="og:title" content="Users Leaderboard | Fantasy Hoops"/>
                <meta name="description" content={Meta.DESCRIPTION}/>
                <meta property="og:description" content={Meta.DESCRIPTION}/>
                <meta name="robots" content="index,follow"/>
                <link rel="canonical" href={Canonicals.USERS_LEADERBOARD}/>
            </Helmet>
            <article className="PageIntro--centered">
                <img
                    src={leaderboardLogo}
                    alt="Leaderboard Logo"
                    width="60rem"
                />
                <h1 className="PageTitle">Top Users</h1>
            </article>
            <div className="row justify-content-center">
                <div className="col-xs">
                    <div style={{transform: 'scale(0.7, 0.7)'}}>
                        <label className="UserLeaderboard__FriendsOnly">
                            <input type="checkbox" checked={friendsOnly} onChange={toggleFriendsOnly}/>
                            <span className="UserLeaderboard__FriendsOnly--slider round"/>
                        </label>
                    </div>
                </div>
                <div className="col-xs pt-2">
                    <div>Friends only</div>
                </div>
            </div>
            <ul className="nav nav-pills justify-content-center mx-auto" id="myTab" role="tablist">
                <li className="nav-item">
                    <a className="nav-link active tab-no-outline" id="daily-tab" data-toggle="tab" href="#daily"
                       role="tab" onClick={switchTab}>Daily</a>
                </li>
                <li className="nav-item">
                    <a className="nav-link tab-no-outline" id="weekly-tab" data-toggle="tab" href="#weekly"
                       role="tab" onClick={switchTab}>Weekly</a>
                </li>
                <li className="nav-item">
                    <a className="nav-link tab-no-outline" id="monthly-tab" data-toggle="tab" href="#monthly"
                       role="tab" onClick={switchTab}>Monthly</a>
                </li>
                <li className="nav-item">
                    <a className="nav-link tab-no-outline" id="fromDate-tab" data-toggle="tab" href="#fromDate"
                       role="tab" onClick={switchTab}>From Date</a>
                </li>
            </ul>
            <div className="tab-content" id="myTabContent">
                <div className="pt-4 pb-1 tab-pane animated bounceInUp show active" id="daily" role="tabpanel">
                    {!loader
                        ? (
                            <div className="DatePicker">
                                <CustomDatePicker
                                    autoOk
                                    type={DatePickerTypes.DAY}
                                    label={"Select date"}
                                    styles={datePickerStyles}
                                    selectedDate={date}
                                    minDate={minDate}
                                    maxDate={maxDate}
                                    onDateChange={onDateChange}
                                />
                            </div>
                        )
                        : null}
                    {!loader
                        ? dailyUsers.length > 0
                            ? dailyUsers
                            : <EmptyJordan message="Such empty..."/>
                        : <div className="Loader"/>}
                    <div className="text-center">
                        {seeMoreButton}
                    </div>
                </div>
                <div className="pt-4 pb-1 tab-pane animated bounceInUp" id="weekly" role="tabpanel">
                    {!loader
                        ? (
                            <div className="WeeklyDatePicker">
                                <CustomDatePicker
                                    autoOk={true}
                                    type={DatePickerTypes.WEEK}
                                    label={"Select week"}
                                    styles={datePickerStyles}
                                    selectedWeek={week}
                                    minDate={minDate}
                                    maxDate={endOfWeek(maxDate, {locale: enLocale})}
                                    onWeekChange={onWeekChange}
                                />
                            </div>
                        )
                        : null}
                    {!loader
                        ? weeklyUsers.length > 0
                            ? weeklyUsers
                            : <EmptyJordan message="Such empty..."/>
                        : <div className="Loader"/>}
                    <div className="text-center">
                        {seeMoreBtn()}
                    </div>
                </div>
                <div className="pt-4 pb-1 tab-pane animated bounceInUp" id="monthly" role="tabpanel">
                    {!loader
                        ? monthlyUsers.length > 0
                            ? monthlyUsers
                            : <EmptyJordan message="Such empty..."/>
                        : <div className="Loader"/>}
                    <div className="text-center">
                        {seeMoreBtn()}
                    </div>
                </div>
                <div className="pt-4 pb-1 tab-pane animated bounceInUp" id="fromDate" role="tabpanel">
                    {!loader
                        ? (
                            <div className="DatePicker">
                                <CustomDatePicker
                                    autoOk
                                    type={DatePickerTypes.DAY}
                                    label={"Select date"}
                                    styles={datePickerStyles}
                                    selectedDate={date}
                                    minDate={minDate}
                                    maxDate={maxDate}
                                    onDateChange={onDateChange}
                                />
                            </div>
                        )
                        : null}
                    {!loader
                        ? fromDateUsers.length > 0
                            ? fromDateUsers
                            : <EmptyJordan message="Such empty..."/>
                        : <div className="Loader"/>}
                    <div className="text-center">
                        {seeMoreBtn()}
                    </div>
                </div>
            </div>
            <PlayerDialog
                renderChild={renderChild}
                loader={modalLoader}
                stats={stats}
                open={playerDialogOpen}
                onDialogOpen={handlePlayerDialogOpen}
                onDialogClose={handlePlayerDialogClose}
            />
        </>
    );
}

export default Leaderboard;
