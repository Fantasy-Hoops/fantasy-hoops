const EC = protractor.ExpectedConditions;
const domain = true
    ? 'https://fantasyhoops.org'
    : 'https://localhost:44389';

// describe('FantasyHoops tournaments page tests', () => {
//     beforeAll(() =>  {
//         browser.waitForAngularEnabled(false);
//         browser.get(`${domain}/achievements`);
//
//         const achievements = element(by.css('.Achievements'));
//         browser.wait(EC.presenceOf(achievements), 10000);
//     });
// });

describe('FantasyHoops login page tests', () => {
    beforeAll(() => {
        browser.waitForAngularEnabled(false);
        browser.get(`${domain}/login`);

        const header = element(by.css('.MuiAppBar-root'));
        browser.wait(EC.presenceOf(header), 10000);
    });

    it('should have H1 title', () => {
        const h1Title = element(by.css('h1'));
        expect(h1Title.isDisplayed()).toBe(true);
    });

    it('should have a working login button', () => {
        const form = element(by.css('#form'));
        browser.wait(EC.presenceOf(form), 10000);

        const username = element(by.css('#username'));
        username.click();
        username.sendKeys("fantasyhoop");

        const password = element(by.css('#password'));
        password.click();
        password.sendKeys("fantasyhoop");

        const loginBtn = element(by.css('#login'));
        loginBtn.click();

        browser.wait(EC.titleIs('Fantasy Hoops | NBA Fantasy Basketball Game'), 5000);

        expect(browser.getCurrentUrl()).toEqual(`${domain}/`);
    });
});

describe('FantasyHoops main page tests', () => {
    beforeAll(() => {
        browser.waitForAngularEnabled(false);
        browser.get(`${domain}/`);

        const header = element(by.css('.MuiAppBar-root'));
        browser.wait(EC.presenceOf(header), 10000);
    });

    it('should have H1 title', () => {
        const h1Title = element(by.css('h1'));
        expect(h1Title.isPresent()).toBe(true);
    });

    it('should have a working "Play Now" button', () => {
        const playNowBtn = element(by.css('#PlayNowBtn'));
        playNowBtn.click();
        browser.wait(EC.titleIs('Lineup | Fantasy Hoops'), 5000);

        expect(browser.getCurrentUrl()).toEqual(`${domain}/lineup`);
    });
});

describe('FantasyHoops lineup page tests', () => {
    beforeAll(() => {
        browser.waitForAngularEnabled(false);
        browser.get(`${domain}/lineup`);

        const playerPool = element(by.css('.PlayerPool'));
        browser.wait(EC.presenceOf(playerPool), 10000);
    });

    it('should have a countdown timer', () => {
        const countdownTimer = element(by.css('.Lineup__countdown'));
        expect(countdownTimer.isDisplayed()).toBe(true);
    })
    
    it('should have lineup selection cards', () => {
        const lineupSelection = element(by.css('.Lineup__body'));
        expect(lineupSelection.isDisplayed()).toBe(true);
    });
    
    it('should have a remaining money element', () => {
        const remaining = element(by.css('.Lineup__moneyRemaining'));
        expect(remaining.isDisplayed()).toBe(true);
    });

    it('should have a price progress bar', () => {
        const progressBar = element(by.css('.Progress__Bar'));
        expect(progressBar.isDisplayed()).toBe(true);
    });

    it('should have a working lineup information dialog', async () => {
        const lineupInfoButton = element(by.css('.Lineup__info-button'));
        await lineupInfoButton.click();

        const lineupInfoDialog = element(by.css('#lineup-info-dialog'));
        browser.wait(EC.presenceOf(lineupInfoDialog), 10000);
        expect(lineupInfoDialog.isDisplayed()).toBe(true);
        
        const lineupInfoDialogCloseBtn = element(by.css('.Lineup__infoDialogClose'));
        await lineupInfoDialogCloseBtn.click();
        browser.wait(EC.invisibilityOf(lineupInfoDialog), 10000);
        expect(lineupInfoDialog.isPresent()).toBe(false);
    });
    
    it('should be able to select player', async () => {
        const lineupInfoDialog = element(by.css('#lineup-info-dialog'));
        browser.wait(EC.invisibilityOf(lineupInfoDialog), 10000);
        
        const playerSelectBtn = element.all(by.css('.PlayerCard__button--circle')).first();
        browser.wait(EC.presenceOf(playerSelectBtn), 10000);
        await playerSelectBtn.click();
        
        const selectedPlayerCard = element.all(by.css('.PlayerCard.card .PlayerCard__player-attributes')).first();
        expect (selectedPlayerCard.isDisplayed()).toBe(true);
    });
    
    it('should be able to open player dialog', async () => {
        const playerName = element.all(by.css('.PlayerCard__player-name')).first();
        await playerName.click();
        
        const playerDialog = element.all(by.css("#PlayerDialog")).first();
        browser.wait(EC.presenceOf(playerDialog), 10000);
        
        expect(playerDialog.isDisplayed()).toBe(true);
    })
});

describe('FantasyHoops leaderboards page tests', () => {
    beforeAll(() =>  {
        browser.waitForAngularEnabled(false);
        browser.get(`${domain}/leaderboards`);

        const usersLeaderboardCard = element.all(by.css('.UserLeaderboardCard')).first();
        browser.wait(EC.presenceOf(usersLeaderboardCard), 10000);
    });

    it('should have H1 title', () => {
        const h1Title = element(by.css('h1'));
        expect(h1Title.isDisplayed()).toBe(true);
    });

    it('should have H5 subtitle', () => {
        const h5Subtitle = element(by.css('h5'));
        expect(h5Subtitle.isDisplayed()).toBe(true);
    });
    
    it('should have valid active users section', () => {
        const activeUsersSection = element(by.css('#Leaderboards__activeUsers'));
        expect(activeUsersSection.isDisplayed()).toBe(true);
        
        const title = element(by.css('#Leaderboards__activeUsers h2'));
        const subtitle = element(by.css('#Leaderboards__activeUsers p'));
        const usersLeaderboardCards = element.all(by.css('#Leaderboards__activeUsers .UserLeaderboardCard'));
        const href = element.all(by.css('#Leaderboards__activeUsers a')).first();
        const link = href.getAttribute('href');
        
        expect(title.isDisplayed()).toBe(true);
        expect(subtitle.isDisplayed()).toBe(true);
        expect(usersLeaderboardCards.count()).toBe(3);
        expect(link).toBe(`${domain}/leaderboard/users`);
    })

    it('should have valid NBA players section', () => {
        const nbaPlayers = element(by.css('#Leaderboards__NBAPlayers'));
        expect(nbaPlayers.isDisplayed()).toBe(true);

        const title = element(by.css('#Leaderboards__NBAPlayers h2'));
        const subtitle = element(by.css('#Leaderboards__NBAPlayers p'));
        const playerLeaderboardCards = element.all(by.css('#Leaderboards__NBAPlayers .PlayerLeaderboardCard'));
        const href = element(by.css('#Leaderboards__NBAPlayers a'));
        const link = href.getAttribute('href');

        expect(title.isDisplayed()).toBe(true);
        expect(subtitle.isDisplayed()).toBe(true);
        expect(playerLeaderboardCards.count()).toBe(3);
        expect(link).toBe(`${domain}/leaderboard/players`);
    })

    it('should have valid best lineups section', () => {
        const bestLineups = element(by.css('#Leaderboards__bestLineups'));
        expect(bestLineups.isDisplayed()).toBe(true);

        const title = element(by.css('#Leaderboards__bestLineups h2'));
        const subtitle = element.all(by.css('#Leaderboards__bestLineups p')).first();
        const userScoreCards = element.all(by.css('#Leaderboards__bestLineups .UserScoreCard'));
        browser.wait(EC.presenceOf(userScoreCards.first()), 10000);
        // const href = element(by.css('#Leaderboards__bestLineups a'));
        // const link = href.getAttribute('href');
        
        expect(title.isDisplayed()).toBe(true);
        expect(subtitle.isDisplayed()).toBe(true);
        expect(userScoreCards.count()).toBe(3);
        // expect(link).toBe(`${domain}/leaderboard/users`);
    })

    it('should have valid season performers section', () => {
        const seasonPerformers = element(by.css('#Leaderboards__seasonPerformers'));
        expect(seasonPerformers.isDisplayed()).toBe(true);

        const title = element(by.css('#Leaderboards__seasonPerformers h2'));
        const subtitle = element(by.css('#Leaderboards__seasonPerformers p'));
        const usersLeaderboardCards = element.all(by.css('#Leaderboards__seasonPerformers .UserLeaderboardCard'));
        const href = element(by.css('#Leaderboards__seasonPerformers .Content__Subtitle + a'));
        const link = href.getAttribute('href');
        
        expect(title.isDisplayed()).toBe(true);
        expect(subtitle.isDisplayed()).toBe(true);
        expect(usersLeaderboardCards.count()).toBe(3);
        expect(link).toBe(`${domain}/leaderboard/season`);
    })

    it('should have valid NBA players selections section', () => {
        const playersSelections = element(by.css('#Leaderboards__NBAPlayersSelections'));
        expect(playersSelections.isDisplayed()).toBe(true);

        const title = element(by.css('#Leaderboards__NBAPlayersSelections h2'));
        const subtitle = element(by.css('#Leaderboards__NBAPlayersSelections p'));
        const playerLeaderboardCards = element.all(by.css('#Leaderboards__NBAPlayersSelections .PlayerLeaderboardCard'));
        browser.wait(EC.presenceOf(playerLeaderboardCards.first()), 10000);
        const href = element(by.css('#Leaderboards__NBAPlayersSelections a'));
        const link = href.getAttribute('href');
        
        expect(title.isDisplayed()).toBe(true);
        expect(subtitle.isDisplayed()).toBe(true);
        expect(playerLeaderboardCards.count()).toBe(3);
        expect(link).toBe(`${domain}/leaderboard/selected/players`);
    })
});

describe('FantasyHoops achievements page tests', () => {
    beforeAll(() => {
        browser.waitForAngularEnabled(false);
        browser.get(`${domain}/achievements`);

        const achievements = element.all(by.css('.AchievementCard__Icon')).first();
        browser.wait(EC.presenceOf(achievements), 10000);
    });

    it('should have H1 title', () => {
        const h1Title = element(by.css('h1'));
        expect(h1Title.isDisplayed()).toBe(true);
    });

    it('should have H5 subtitle', () => {
        const h1Title = element(by.css('h5'));
        expect(h1Title.isDisplayed()).toBe(true);
    });

    it('should have a clickable achievement card', async () => {
        const achievementCard = element.all(by.css('.AchievementCard__Icon')).first();
        achievementCard.click();

        const achievementDialog = element(by.css('.AchievementDialog__Paper'));
        browser.wait(EC.presenceOf(achievementDialog), 5000);

        expect(achievementDialog.isDisplayed()).toBe(true);
    });

    it('should have an achievement card title', async () => {
        const achievementDialogTitle = element(by.css('.AchievementDialog__Title'));
        expect((await achievementDialogTitle.getText()).length).toBeGreaterThan(0);
    });

    it('should have an achievement card description', async () => {
        const achievementDialogDescription = element(by.css('.AchievementDialog__Description'));
        expect((await achievementDialogDescription.getText()).length).toBeGreaterThan(0);
    });

    it('should be able to close achievement dialog', () => {
        const achievementDialog = element(by.css('.AchievementDialog__Paper'));
        const closeBtn = element(by.css('.AchievementDialog__CloseIcon'));
        closeBtn.click();

        browser.wait(EC.invisibilityOf(achievementDialog), 5000);

        expect(achievementDialog.isPresent()).toBe(false);
    });
});

describe('FantasyHoops injuries page tests', () => {
    beforeAll(() => {
        browser.waitForAngularEnabled(false);
        browser.get(`${domain}/injuries`);

        const injuryCard = element.all(by.css('.InjuryCardContainer')).first();
        browser.wait(EC.presenceOf(injuryCard), 10000);
    });

    it('should have H1 title', () => {
        const h1Title = element(by.css('h1'));
        expect(h1Title.isDisplayed()).toBe(true);
    });

    it('should have H5 subtitle', () => {
        const h5Subtitle = element(by.css('h5'));
        expect(h5Subtitle.isDisplayed()).toBe(true);
    });
    
    it('should have at least 10 injury cars', () => {
        const injuryCards = element.all(by.css('.InjuryCardContainer'));
        expect(injuryCards.count()).toBeGreaterThan(9);
    });
    
    it('should have working injuries info dialog button', async () => {
        const injuriesInfoBtn = element(by.css('.Injuries__InfoButton button'));
        expect(injuriesInfoBtn.isDisplayed()).toBe(true);
        await injuriesInfoBtn.click();
        
        const injuriesInfoDialog = element(by.css('.InjuriesInfoDialog'));
        browser.wait(EC.presenceOf(injuriesInfoDialog), 1000);
        expect(injuriesInfoDialog.isDisplayed()).toBe(true);

        const injuriesInfoDialogCloseBtn = element(by.css('.InjuriesInfoDialog button'));
        await injuriesInfoDialogCloseBtn.click();
        browser.wait(EC.invisibilityOf(injuriesInfoDialog), 1000);
        expect(injuriesInfoDialog.isPresent()).toBe(false);
    });

    it('should have working player injury info dialog', async () => {
        const injuryCard = element.all(by.css('.InjuryCardContainer')).last();
        await injuryCard.click();
        
        const playerInjuryDialog = element(by.css('#InjuryPlayerDialog'));
        browser.wait(EC.presenceOf(playerInjuryDialog), 10000);
        expect(playerInjuryDialog.isDisplayed()).toBe(true);

        const injuriesInfoDialogCloseBtn = element(by.css('#InjuryPlayerDialog button'));
        await injuriesInfoDialogCloseBtn.click();
        browser.wait(EC.invisibilityOf(playerInjuryDialog), 1000);
        expect(playerInjuryDialog.isPresent()).toBe(false);
    });
});

describe('FantasyHoops news page tests', () => {
    beforeAll(() => {
        browser.waitForAngularEnabled(false);
        browser.get(`${domain}/news`);

        const newsCard = element.all(by.css('.NewsCard')).first();
        browser.wait(EC.presenceOf(newsCard), 10000);
    });

    it('should have H1 title', () => {
        const h1Title = element(by.css('h1'));
        expect(h1Title.isDisplayed()).toBe(true);
    });

    it('should have H5 subtitle', () => {
        const h5Subtitle = element.all(by.css('h5')).first();
        expect(h5Subtitle.isDisplayed()).toBe(true);
    });
    
    it('should have two news tabs', () => {
        const tabs = element.all(by.css('#NewsFeed__tabs .MuiTab-root'));
        expect(tabs.count()).toBe(2);
    });
    
    it('should have at least 3 cards in previews section', () => {
        const previewsPanel = element(by.css('#news-tabpanel-0'));
        expect(previewsPanel.isDisplayed()).toBe(true);
        
        const previewsCards = element.all(by.css('#news-tabpanel-0 .NewsCard'));
        expect(previewsCards.count()).toBeGreaterThan(2);
    });
    
    it('should have working recaps tab', async () => {
        const recapsTab = element.all(by.css('#NewsFeed__tabs .MuiTab-root')).last();
        await recapsTab.click();
        const recapsPanel = element(by.css('#news-tabpanel-1'));
        browser.wait(EC.presenceOf(recapsPanel), 10000);
        
        expect(recapsPanel.isDisplayed()).toBe(true);
    });

    it('should have at least 3 cards in recaps section', () => {
        const recapsPanel = element(by.css('#news-tabpanel-1'));
        expect(recapsPanel.isDisplayed()).toBe(true);
        browser.wait(EC.visibilityOf(recapsPanel), 10000);

        const recapsCard = element.all(by.css('#news-tabpanel-1 .NewsCard'));
        expect(recapsCard.count()).toBeGreaterThan(2);
    });
});